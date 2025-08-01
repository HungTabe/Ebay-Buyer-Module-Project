using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class CreateReturnRequestPageModel : PageModel
    {
        private readonly IOrderService _orderService;

        public CreateReturnRequestPageModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [BindProperty]
        public CreateReturnRequestModel ReturnRequest { get; set; } = new();

        public List<OrderSummaryViewModel> AvailableOrders { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int? orderId)
        {
            // Get user ID from session (you may need to adjust this based on your authentication)
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");
            

            await LoadAvailableOrders(userId.Value);
            
            // Pre-select order if orderId is provided
            if (orderId.HasValue && AvailableOrders.Any(o => o.Id == orderId.Value))
            {
                ReturnRequest.OrderId = orderId.Value;
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");
            

            if (!ModelState.IsValid)
            {
                await LoadAvailableOrders(userId.Value);
                return Page();
            }

            try
            {
                var success = await _orderService.CreateReturnRequestAsync(ReturnRequest, userId.Value);
                
                if (success)
                {
                    SuccessMessage = "Return request submitted successfully! We will review your request and contact you soon.";
                    ReturnRequest = new CreateReturnRequestModel();
                    await LoadAvailableOrders(userId.Value);
                }
                else
                {
                    ErrorMessage = "Unable to create return request. Please make sure the order is eligible for return and you haven't already submitted a request for this order.";
                    await LoadAvailableOrders(userId.Value);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while processing your request. Please try again.";
                await LoadAvailableOrders(userId.Value);
            }

            return Page();
        }

        private async Task LoadAvailableOrders(int userId)
        {
            try
            {
                var orderHistory = await _orderService.GetOrderHistoryAsync(userId, 1, 100); // Get more orders to check eligibility
                var availableOrders = new List<OrderSummaryViewModel>();

                foreach (var order in orderHistory.Orders)
                {
                    if (await _orderService.CanReturnOrderAsync(order.Id, userId))
                    {
                        availableOrders.Add(order);
                    }
                }

                AvailableOrders = availableOrders;
            }
            catch
            {
                AvailableOrders = new List<OrderSummaryViewModel>();
            }
        }
    }
} 