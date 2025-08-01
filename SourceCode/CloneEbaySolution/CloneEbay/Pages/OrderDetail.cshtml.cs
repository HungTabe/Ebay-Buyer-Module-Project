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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            OrderDetail = await _orderService.GetOrderDetailAsync(id, userId.Value);
            
            if (OrderDetail == null)
                return NotFound();

            // Check if order can be returned
            if (OrderDetail.Status?.ToLower() == "delivered")
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