using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Interfaces;
using CloneEbay.Models;

namespace CloneEbay.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IProductService _productService;

        public ProductsModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public ProductFilterModel Filter { get; set; } = new();

        public ProductListViewModel ProductList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                ProductList = await _productService.GetProductsAsync(Filter);
                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception in a real application
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Reset to first page when applying new filters
            Filter.Page = 1;
            return await OnGetAsync();
        }
    }
} 