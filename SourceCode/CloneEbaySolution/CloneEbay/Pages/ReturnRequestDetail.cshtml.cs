using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class ReturnRequestDetailModel : PageModel
    {
        private readonly IOrderService _orderService;

        public ReturnRequestDetailModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public ReturnRequestViewModel? ReturnRequest { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            try
            {
                ReturnRequest = await _orderService.GetReturnRequestAsync(id, userId.Value);
                
                if (ReturnRequest == null)
                {
                    return NotFound();
                }
            }
            catch
            {
                return NotFound();
            }

            return Page();
        }
    }
} 