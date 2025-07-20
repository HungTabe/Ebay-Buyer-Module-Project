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

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            // Confirm delivery
            DeliveryConfirmed = await _orderService.ConfirmOrderDeliveredAsync(orderId, userId.Value);

            // Get order details for display
            Order = await _orderService.GetOrderDetailAsync(orderId, userId.Value);

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