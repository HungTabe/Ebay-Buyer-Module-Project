using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class ReturnRequestsModel : PageModel
    {
        private readonly IOrderService _orderService;

        public ReturnRequestsModel(IOrderService orderService)
        {
            _orderService = orderService;
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

        public ReturnRequestListViewModel? ReturnRequests { get; set; }

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            try
            {
                ReturnRequests = await _orderService.GetReturnRequestsAsync(userId.Value, page, 12);
            }
            catch
            {
                ReturnRequests = new ReturnRequestListViewModel();
            }

            return Page();
        }
    }
} 