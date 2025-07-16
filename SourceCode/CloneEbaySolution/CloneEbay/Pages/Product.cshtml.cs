using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Interfaces;
using CloneEbay.Models;

namespace CloneEbay.Pages
{
    public class ProductModel : PageModel
    {
        private readonly IProductService _productService;

        public ProductModel(IProductService productService)
        {
            _productService = productService;
        }

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
    }
} 