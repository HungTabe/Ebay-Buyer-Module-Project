using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Data;
using CloneEbay.Data.Entities;
using CloneEbay.Models;
using CloneEbay.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CloneEbay.Pages
{
    public class OrderModel : PageModel
    {
        private readonly CloneEbayDbContext _db;
        private readonly CartService _cartService;

        public OrderModel(CloneEbayDbContext db, CartService cartService)
        {
            _db = db;
            _cartService = cartService;
        }

        public List<CartItem> CartItems { get; set; } = new();
        public decimal TotalPrice => CartItems.Sum(x => x.Price * x.Quantity);
        public List<Address> Addresses { get; set; } = new();

        [BindProperty]
        public int SelectedAddressId { get; set; }
        [BindProperty]
        public Address NewAddress { get; set; } = new();
        [BindProperty]
        public bool AddNewAddress { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");
            Addresses = await _db.Addresses.Where(a => a.UserId == userId).ToListAsync();
            CartItems = _cartService.GetCartItems();
            if (Addresses.Any(a => a.IsDefault == true))
                SelectedAddressId = Addresses.First(a => a.IsDefault == true).Id;
            return Page();
        }

        public async Task<IActionResult> OnPostAddAddressAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");
            NewAddress.UserId = userId;
            if (NewAddress.IsDefault == true)
            {
                // Bỏ mặc định các địa chỉ khác
                var addresses = await _db.Addresses.Where(a => a.UserId == userId).ToListAsync();
                foreach (var addr in addresses)
                {
                    addr.IsDefault = false;
                }
            }
            _db.Addresses.Add(NewAddress);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPlaceOrderAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");
            CartItems = _cartService.GetCartItems();
            if (CartItems.Count == 0)
                return RedirectToPage("/Cart");
            var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == SelectedAddressId && a.UserId == userId);
            if (address == null)
            {
                ModelState.AddModelError("", "Vui lòng chọn địa chỉ giao hàng.");
                Addresses = await _db.Addresses.Where(a => a.UserId == userId).ToListAsync();
                return Page();
            }
            var order = new OrderTable
            {
                BuyerId = userId,
                AddressId = address.Id,
                OrderDate = DateTime.UtcNow,
                TotalPrice = CartItems.Sum(x => x.Price * x.Quantity),
                Status = "Pending"
            };
            _db.OrderTables.Add(order);
            await _db.SaveChangesAsync();
            foreach (var item in CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                _db.OrderItems.Add(orderItem);
            }
            await _db.SaveChangesAsync();
            _cartService.ClearCart();
            return RedirectToPage("/Payment", new { orderId = order.Id });
        }

        private int? GetUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    return userId;
            }
            return null;
        }
    }
} 