using CloneEbay.Models;

namespace CloneEbay.Interfaces
{
    public interface IProductService
    {
        Task<ProductListViewModel> GetProductsAsync(ProductFilterModel filter);
        Task<List<CategoryViewModel>> GetCategoriesAsync();
        Task<ProductViewModel?> GetProductByIdAsync(int id);
        Task<ProductReviewViewModel> GetProductReviewsAsync(int productId);
        Task<bool> AddProductReviewAsync(ReviewModel review, int userId);
        Task<bool> HasUserReviewedProductAsync(int productId, int userId);
        Task<bool> CanUserReviewProductAsync(int productId, int userId);
    }
} 