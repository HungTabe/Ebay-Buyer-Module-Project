using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class OrderHistoryModel : PageModel
    {
        private readonly IOrderService _orderService;

        public OrderHistoryModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public OrderHistoryViewModel OrderHistory { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            OrderHistory = await _orderService.GetOrderHistoryAsync(userId.Value, Page, 12);
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