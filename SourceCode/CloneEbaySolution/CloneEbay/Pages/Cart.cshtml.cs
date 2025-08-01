using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Models;
using CloneEbay.Services;
using System.Collections.Generic;

namespace CloneEbay.Pages
{
    public class CartModel : PageModel
    {
        private readonly CartService _cartService;
        public CartModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public List<CartItem> CartItems { get; set; } = new();
        [BindProperty]
        public int UpdateProductId { get; set; }
        [BindProperty]
        public int UpdateQuantity { get; set; }

        public void OnGet()
        {
            CartItems = _cartService.GetCartItems();
        }

        public IActionResult OnPostUpdateQuantity()
        {
            if (UpdateProductId > 0 && UpdateQuantity > 0)
            {
                _cartService.UpdateQuantity(UpdateProductId, UpdateQuantity);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToPage();
        }
    }
} 