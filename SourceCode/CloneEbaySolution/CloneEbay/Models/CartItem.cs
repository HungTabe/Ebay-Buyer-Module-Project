using System;

namespace CloneEbay.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        // Optional: For coupon logic, to show discounted price per item
        public decimal? DiscountedPrice { get; set; }
    }
} 