using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Data;
using CloneEbay.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace CloneEbay.Pages
{
    public class PaymentModel : PageModel
    {
        private readonly CloneEbayDbContext _db;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public string StripePublishableKey { get; set; }
        public PaymentModel(CloneEbayDbContext db, IPaymentService paymentService, IEmailService emailService, IConfiguration configuration)
        {
            _db = db;
            _paymentService = paymentService;
            _emailService = emailService;
            _configuration = configuration;
            StripePublishableKey = _configuration["Stripe:PublishableKey"];
        }

        public OrderTable? Order { get; set; }
        public string? CouponCode { get; set; }
        public decimal? CouponDiscountPercent { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            Order = await _db.OrderTables
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Address)
                .Include(o => o.Coupon)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (Order == null)
                return RedirectToPage("/Error");
            if (Order.Coupon != null)
            {
                CouponCode = Order.Coupon.Code;
                CouponDiscountPercent = Order.Coupon.DiscountPercent;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostPayAsync(int orderId, string stripeToken)
        {
            var order = await _db.OrderTables
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Address)
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return RedirectToPage("/Error");

            // Calculate amount in cents
            var amount = (long)((order.TotalPrice ?? 0) * 100);
            var currency = "usd";
            var description = $"Order #{order.Id} payment";

            var paymentSuccess = await _paymentService.ProcessPaymentAsync(amount, currency, stripeToken, description);
            if (!paymentSuccess)
            {
                ErrorMessage = "Payment failed. Please try again.";
                await OnGetAsync(orderId); // reload order info
                return Page();
            }

            order.Status = "Paid";
            await _db.SaveChangesAsync();

            // Send email to user (get from Buyer)
            var userEmail = order.Buyer?.Email ?? "";
            var subject = $"Order #{order.Id} Payment Confirmation";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border:1px solid #eee; border-radius:8px; overflow:hidden;'>
                    <div style='background: #0064d2; color: #fff; padding: 24px 16px; text-align: center;'>
                        <h1 style='margin:0; font-size:40px; font-weight:bold; color:#fff; letter-spacing:2px;'>eBay</h1>
                        <h2 style='margin:0;'>Thank you for your payment!</h2>
                    </div>
                    <div style='padding: 24px 16px; background: #f9f9f9;'>
                        <h3 style='color:#222;'>Order Confirmation <span style='color:#0064d2;'>#{order.Id}</span></h3>
                        <p style='margin:0 0 16px 0;'>Hi <b>{order.Buyer?.Username ?? "Customer"}</b>,<br>We have received your payment. Your order is now being processed.</p>
                        <table style='width:100%; border-collapse:collapse; margin-bottom:16px;'>
                            <thead>
                                <tr style='background:#eee;'>
                                    <th style='padding:8px; border:1px solid #ddd; text-align:left;'>Product</th>
                                    <th style='padding:8px; border:1px solid #ddd;'>Qty</th>
                                    <th style='padding:8px; border:1px solid #ddd;'>Unit Price</th>
                                    <th style='padding:8px; border:1px solid #ddd;'>Subtotal</th>
                                </tr>
                            </thead>
                            <tbody>
                                {string.Join("", order.OrderItems.Select(item => $@"<tr>
                                    <td style='padding:8px; border:1px solid #ddd;'>{item.Product?.Title}</td>
                                    <td style='padding:8px; border:1px solid #ddd; text-align:center;'>{item.Quantity}</td>
                                    <td style='padding:8px; border:1px solid #ddd;'>${item.UnitPrice?.ToString("F2")}</td>
                                    <td style='padding:8px; border:1px solid #ddd;'>${((item.UnitPrice ?? 0) * (item.Quantity ?? 0)).ToString("F2")}</td>
                                </tr>"))}
                            </tbody>
                        </table>
                        <div style='margin-bottom:16px;'>
                            <b>Total Paid:</b> <span style='color:#0064d2;'>${order.TotalPrice?.ToString("F2")}</span><br/>
                            <b>Order Date:</b> {order.OrderDate?.ToString("dd/MM/yyyy HH:mm")}
                        </div>
                        <div style='margin-bottom:16px;'>
                            <b>Shipping Address:</b><br/>
                            {order.Address?.FullName}<br/>
                            {order.Address?.Street}, {order.Address?.City}, {order.Address?.State}, {order.Address?.Country}, {order.Address?.PostalCode}<br/>
                            Phone: {order.Address?.Phone}
                        </div>
                        <div style='margin-bottom:16px;'>
                            <b>Status:</b> <span style='color:green;'>Paid</span>
                        </div>
                        <div style='background:#e6f7ff; padding:12px; border-radius:6px; margin-bottom:16px;'>
                            <b>Need help?</b> Contact our support at <a href='mailto:support@ebay.com'>support@ebay.com</a>.
                        </div>
                        <div style='text-align:center; color:#888; font-size:13px;'>
                            &copy; {DateTime.Now.Year} eBay. This is a payment confirmation email. Please do not reply directly to this email.
                        </div>
                    </div>
                </div>
            ";
            if (!string.IsNullOrEmpty(userEmail))
            {
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }

            // Redirect to order confirmation page
            return RedirectToPage("/OrderConfirmation", new { orderId = order.Id });
        }
    }
} 