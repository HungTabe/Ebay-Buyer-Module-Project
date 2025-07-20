using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class ProductReviewsModel : PageModel
    {
        private readonly IProductService _productService;

        public ProductReviewsModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public ProductReviewViewModel ProductReview { get; set; } = new();

        [BindProperty]
        public ReviewModel NewReview { get; set; } = new();

        public bool IsAuthenticated { get; set; }
        public bool HasUserReviewed { get; set; }
        public bool CanReview { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ProductReview = await _productService.GetProductReviewsAsync(id);
            
            if (ProductReview.ProductId == 0)
            {
                return RedirectToPage("/Error");
            }

            IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
            
            if (IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                HasUserReviewed = await _productService.HasUserReviewedProductAsync(id, userId);
                CanReview = await _productService.CanUserReviewProductAsync(id, userId);
            }
            else
            {
                CanReview = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Auth/Login");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (userId == 0)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Check eligibility before allowing review
            CanReview = await _productService.CanUserReviewProductAsync(NewReview.ProductId, userId);
            if (!CanReview)
            {
                TempData["ErrorMessage"] = "You can only review this product after your order has been delivered.";
                ProductReview = await _productService.GetProductReviewsAsync(NewReview.ProductId);
                IsAuthenticated = true;
                HasUserReviewed = await _productService.HasUserReviewedProductAsync(NewReview.ProductId, userId);
                return Page();
            }

            if (!ModelState.IsValid)
            {
                ProductReview = await _productService.GetProductReviewsAsync(NewReview.ProductId);
                IsAuthenticated = true;
                HasUserReviewed = await _productService.HasUserReviewedProductAsync(NewReview.ProductId, userId);
                return Page();
            }

            // Validate rating
            if (NewReview.Rating < 1 || NewReview.Rating > 5)
            {
                ModelState.AddModelError("NewReview.Rating", "Rating must be between 1 and 5 stars.");
                ProductReview = await _productService.GetProductReviewsAsync(NewReview.ProductId);
                IsAuthenticated = true;
                HasUserReviewed = await _productService.HasUserReviewedProductAsync(NewReview.ProductId, userId);
                return Page();
            }

            // Validate comment
            if (string.IsNullOrWhiteSpace(NewReview.Comment))
            {
                ModelState.AddModelError("NewReview.Comment", "Comment is required.");
                ProductReview = await _productService.GetProductReviewsAsync(NewReview.ProductId);
                IsAuthenticated = true;
                HasUserReviewed = await _productService.HasUserReviewedProductAsync(NewReview.ProductId, userId);
                return Page();
            }

            var success = await _productService.AddProductReviewAsync(NewReview, userId);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Your review has been submitted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to submit review. Please try again.";
            }

            return RedirectToPage("/ProductReviews", new { id = NewReview.ProductId });
        }
    }
} 