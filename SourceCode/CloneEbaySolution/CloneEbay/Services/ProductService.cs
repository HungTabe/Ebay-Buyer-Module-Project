using CloneEbay.Data;
using CloneEbay.Data.Entities;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.EntityFrameworkCore;

namespace CloneEbay.Services
{
    public class ProductService : IProductService
    {
        private readonly CloneEbayDbContext _context;

        public ProductService(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<ProductListViewModel> GetProductsAsync(ProductFilterModel filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Bids)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(p => p.Title!.Contains(filter.SearchTerm) || 
                                        p.Description!.Contains(filter.SearchTerm));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice);
            }

            if (filter.IsAuction)
            {
                query = query.Where(p => p.IsAuction == true);
            }

            if (!string.IsNullOrEmpty(filter.Condition))
            {
                query = query.Where(p => p.Condition == filter.Condition);
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(p => p.Title)
                    : query.OrderBy(p => p.Title),
                "price" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "date" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            // Apply pagination
            var products = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Convert to view models
            var productViewModels = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Title = p.Title ?? "",
                Description = p.Description ?? "",
                Price = p.Price ?? 0,
                Images = p.Images,
                CategoryName = p.Category?.Name ?? "",
                SellerName = p.Seller?.Username ?? "",
                IsAuction = p.IsAuction ?? false,
                AuctionEndTime = p.AuctionEndTime,
                Condition = p.Condition ?? "",
                CreatedAt = p.CreatedAt ?? DateTime.UtcNow,
                BidCount = p.Bids?.Count ?? 0,
                CurrentBid = p.Bids?.OrderByDescending(b => b.Amount).FirstOrDefault()?.Amount
            }).ToList();

            return new ProductListViewModel
            {
                Products = productViewModels,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = filter.Page,
                Filter = filter,
                Categories = await GetCategoriesAsync()
            };
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name ?? "",
                ParentId = c.ParentId,
                ProductCount = c.Products?.Count ?? 0
            }).ToList();
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Bids)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            return new ProductViewModel
            {
                Id = product.Id,
                Title = product.Title ?? "",
                Description = product.Description ?? "",
                Price = product.Price ?? 0,
                Images = product.Images,
                CategoryName = product.Category?.Name ?? "",
                SellerName = product.Seller?.Username ?? "",
                IsAuction = product.IsAuction ?? false,
                AuctionEndTime = product.AuctionEndTime,
                Condition = product.Condition ?? "",
                CreatedAt = product.CreatedAt ?? DateTime.UtcNow,
                BidCount = product.Bids?.Count ?? 0,
                CurrentBid = product.Bids?.OrderByDescending(b => b.Amount).FirstOrDefault()?.Amount
            };
        }

        public async Task<ProductReviewViewModel> GetProductReviewsAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return new ProductReviewViewModel();

            var reviews = await _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reviewViewModels = reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                ProductId = r.ProductId ?? 0,
                ReviewerId = r.ReviewerId ?? 0,
                ReviewerName = r.Reviewer?.Username ?? "Anonymous",
                Rating = r.Rating ?? 0,
                Comment = r.Comment ?? "",
                CreatedAt = r.CreatedAt ?? DateTime.UtcNow
            }).ToList();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating ?? 0) : 0;

            return new ProductReviewViewModel
            {
                ProductId = productId,
                ProductTitle = product.Title ?? "",
                Reviews = reviewViewModels,
                AverageRating = Math.Round(averageRating, 1),
                TotalReviews = reviews.Count,
                NewReview = new ReviewModel { ProductId = productId }
            };
        }

        public async Task<bool> AddProductReviewAsync(ReviewModel review, int userId)
        {
            try
            {
                // Check if user has already reviewed this product
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.ProductId == review.ProductId && r.ReviewerId == userId);

                if (existingReview != null)
                {
                    // Update existing review
                    existingReview.Rating = review.Rating;
                    existingReview.Comment = review.Comment;
                    existingReview.CreatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new review
                    var newReview = new Review
                    {
                        ProductId = review.ProductId,
                        ReviewerId = userId,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Reviews.Add(newReview);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasUserReviewedProductAsync(int productId, int userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ProductId == productId && r.ReviewerId == userId);
        }

        public async Task<bool> CanUserReviewProductAsync(int productId, int userId)
        {
            // Check if the user has an order for the product with status 'delivered'
            return await _context.OrderItems
                .Where(oi => oi.ProductId == productId)
                .AnyAsync(oi => oi.Order != null && oi.Order.BuyerId == userId && oi.Order.Status != null && oi.Order.Status.ToLower() == "delivered");
        }
    }
} 