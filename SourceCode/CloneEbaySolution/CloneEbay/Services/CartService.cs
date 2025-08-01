using CloneEbay.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace CloneEbay.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "CartItems";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cart = session.GetObjectFromJson<List<CartItem>>(SessionKey);
            return cart ?? new List<CartItem>();
        }

        public void AddToCart(CartItem item)
        {
            var cart = GetCartItems();
            var existing = cart.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            SaveCart(new List<CartItem>());
        }

        private void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetObjectAsJson(SessionKey, cart);
        }
    }
} 