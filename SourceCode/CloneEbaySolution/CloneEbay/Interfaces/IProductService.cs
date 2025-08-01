using CloneEbay.Models;

namespace CloneEbay.Interfaces
{
    public interface IProductService
    {
        Task<ProductListViewModel> GetProductsAsync(ProductFilterModel filter);
        Task<List<CategoryViewModel>> GetCategoriesAsync();
        Task<ProductViewModel?> GetProductByIdAsync(int id);
    }
} 