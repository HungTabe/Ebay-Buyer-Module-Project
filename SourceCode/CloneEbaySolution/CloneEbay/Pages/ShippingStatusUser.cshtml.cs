using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Data.Entities;
using CloneEbay.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class ShippingStatusUserModel : PageModel
    {
        private readonly IShippingInfoService _shippingInfoService;
        private readonly IEmailService _emailService;
        public ShippingStatusUserModel(IShippingInfoService shippingInfoService, IEmailService emailService)
        {
            _shippingInfoService = shippingInfoService;
            _emailService = emailService;
        }

        public List<ShippingInfo> Shipments { get; set; } = new();

        [BindProperty]
        public int UpdateShipmentId { get; set; }
        [BindProperty]
        public string UpdateStatus { get; set; } = string.Empty;

        // Search properties
        [BindProperty(SupportsGet = true)]
        public string? SearchOrderId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchTrackingNumber { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchStatus { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchEstimatedArrival { get; set; }

        public async Task OnGetAsync()
        {
            int? userId = GetUserId();
            if (userId == null)
            {
                Shipments = new List<ShippingInfo>();
                return;
            }
            if (!string.IsNullOrWhiteSpace(SearchOrderId) || !string.IsNullOrWhiteSpace(SearchTrackingNumber) || !string.IsNullOrWhiteSpace(SearchStatus) || !string.IsNullOrWhiteSpace(SearchEstimatedArrival))
            {
                Shipments = await _shippingInfoService.SearchUserShipmentsAsync(userId.Value, SearchOrderId, SearchTrackingNumber, SearchStatus, SearchEstimatedArrival);
            }
            else
            {
                Shipments = await _shippingInfoService.GetUserShipmentsAsync(userId.Value);
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync()
        {
            int? userId = GetUserId();
            if (userId == null)
                return RedirectToPage();


            var updated = await _shippingInfoService.UpdateShipmentStatusAsync(UpdateShipmentId, userId.Value, UpdateStatus);


            if (updated)
            {
                // Lấy lại shipment và order để gửi email
                var shipment = (await _shippingInfoService.GetUserShipmentsAsync(userId.Value)).FirstOrDefault(s => s.Id == UpdateShipmentId);
                if (shipment != null && shipment.Order != null && shipment.Order.Buyer != null)
                {
                    var userEmail = shipment.Order.Buyer.Email;
                    var subject = $"Order #{shipment.Order.Id} Shipping Status Update";
                    var body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border:1px solid #eee; border-radius:8px; overflow:hidden;'>
                            <div style='background: #0064d2; color: #fff; padding: 24px 16px; text-align: center;'>
                                <h1 style='margin:0; font-size:40px; font-weight:bold; color:#fff; letter-spacing:2px;'>ebay</h1>
                                <h2 style='margin:0;'>Shipping Status Update</h2>
                            </div>
                            <div style='padding: 24px 16px; background: #f9f9f9;'>
                                <h3 style='color:#222;'>Order <span style='color:#0064d2;'>#{shipment.Order.Id}</span></h3>
                                <p>Hi <b>{shipment.Order.Buyer.Username}</b>,<br>Your shipment status has been updated to <b>{shipment.Status}</b>.</p>
                                <p><b>Shipping Carrier:</b> {shipment.Carrier}</p>
                                <p><b>Tracking Number:</b> {shipment.TrackingNumber}</p>
                                <p><b>Estimated Arrival:</b> {shipment.EstimatedArrival?.ToString("dd/MM/yyyy")}</p>
                                <div style='margin-bottom:16px;'>
                                    <b>Shipping Address:</b><br/>
                                    {shipment.Order.Address?.FullName}<br/>
                                    {shipment.Order.Address?.Street}, {shipment.Order.Address?.City}, {shipment.Order.Address?.State}, {shipment.Order.Address?.Country}, {shipment.Order.Address?.PostalCode}<br/>
                                    Phone: {shipment.Order.Address?.Phone}
                                </div>
                                <div style='margin-bottom:16px;'>
                                    <b>Order Status:</b> <span style='color:green;'>{shipment.Order.Status}</span>
                                </div>
                                <div style='background:#e6f7ff; padding:12px; border-radius:6px; margin-bottom:16px;'>
                                    <b>Need help?</b> Contact our support at <a href='mailto:support@ebay.com'>support@ebay.com</a>.
                                </div>
                                <div style='text-align:center; color:#888; font-size:13px;'>
                                    &copy; {System.DateTime.Now.Year} eBay. This is a shipping status update email. Please do not reply directly to this email.
                                </div>
                            </div>
                        </div>
                    ";
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        await _emailService.SendEmailAsync(userEmail, subject, body);
                    }
                }
            }
            return RedirectToPage();
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