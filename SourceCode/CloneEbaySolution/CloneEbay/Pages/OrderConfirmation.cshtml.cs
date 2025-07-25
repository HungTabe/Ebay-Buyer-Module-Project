using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly IOrderService _orderService;

        public OrderConfirmationModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public OrderDetailViewModel? Order { get; set; }
        public bool DeliveryConfirmed { get; set; }
        public string? CouponCode { get; set; }
        public decimal? CouponDiscountPercent { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal CouponDiscount { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            // Confirm delivery
            //DeliveryConfirmed = await _orderService.ConfirmOrderDeliveredAsync(orderId, userId.Value);

            // Get order details for display
            Order = await _orderService.GetOrderDetailAsync(orderId, userId.Value);

            // Get coupon information if available
            if (Order != null)
            {
                // We need to get coupon info from the database since OrderDetailViewModel doesn't include it
                var orderWithCoupon = await _orderService.GetOrderWithCouponAsync(orderId, userId.Value);
                if (orderWithCoupon?.Coupon != null)
                {
                    CouponCode = orderWithCoupon.Coupon.Code;
                    CouponDiscountPercent = orderWithCoupon.Coupon.DiscountPercent;
                }

                // Tính Subtotal
                Subtotal = Order.Items.Sum(i => i.TotalPrice);
                // Tính ShippingFee
                var region = Order.Address != null ? Order.Address.GetRegion() : "Unknown";
                ShippingFee = CloneEbay.Services.ShippingFeeCalculator.Calculate(region);
                // Tính CouponDiscount
                CouponDiscount = 0;
                if (CouponDiscountPercent > 0)
                {
                    CouponDiscount = Subtotal * (CouponDiscountPercent.Value / 100);
                }
            }

            return Page();
        }

        private int? GetUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    return userId;
            }
            return null;
        }
    }
} 