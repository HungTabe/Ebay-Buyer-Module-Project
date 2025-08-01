using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class OrderDetailModel : PageModel
    {
        private readonly IOrderService _orderService;

        public OrderDetailModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public OrderDetailViewModel? OrderDetail { get; set; }
        public bool CanReturnOrder { get; set; }
        public bool HasReturnRequest { get; set; }
        public string? CouponCode { get; set; }
        public decimal? CouponDiscountPercent { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            OrderDetail = await _orderService.GetOrderDetailAsync(id, userId.Value);
            
            if (OrderDetail == null)
                return NotFound();

            // Get coupon information if available
            var orderWithCoupon = await _orderService.GetOrderWithCouponAsync(id, userId.Value);
            if (orderWithCoupon?.Coupon != null)
            {
                CouponCode = orderWithCoupon.Coupon.Code;
                CouponDiscountPercent = orderWithCoupon.Coupon.DiscountPercent;
            }

            // Check if order can be returned (delivered or paid+shipment delivered)
            var status = OrderDetail.Status?.ToLower();
            var shippingStatus = OrderDetail.ShippingInfo?.Status?.ToLower();
            var canReviewOrReturn = status == "delivered" || (status == "paid" && shippingStatus == "delivered");
            if (canReviewOrReturn)
            {
                CanReturnOrder = await _orderService.CanReturnOrderAsync(id, userId.Value);
                HasReturnRequest = !CanReturnOrder && await _orderService.GetReturnRequestsAsync(userId.Value, 1, 100)
                    .ContinueWith(t => t.Result.ReturnRequests.Any(r => r.OrderId == id));
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