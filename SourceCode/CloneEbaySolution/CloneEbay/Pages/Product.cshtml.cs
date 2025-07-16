using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using CloneEbay.Services;

namespace CloneEbay.Pages
{
    public class ProductModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly CartService _cartService;

        public ProductModel(IProductService productService, CartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        [BindProperty]
        public int Quantity { get; set; } = 1;

        public ProductViewModel? Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
            
            if (Product == null)
            {
                return RedirectToPage("/Error");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
            if (Product == null)
            {
                return RedirectToPage("/Error");
            }
            var cartItem = new CartItem
            {
                ProductId = Product.Id,
                ProductName = Product.Title,
                Price = Product.Price,
                Quantity = Quantity,
                ImageUrl = Product.Images?.Split(',').FirstOrDefault() ?? ""
            };
            _cartService.AddToCart(cartItem);
            TempData["CartMessage"] = $"Đã thêm {Product.Title} vào giỏ hàng!";
            return RedirectToPage("/Cart");
        }
    }
} 