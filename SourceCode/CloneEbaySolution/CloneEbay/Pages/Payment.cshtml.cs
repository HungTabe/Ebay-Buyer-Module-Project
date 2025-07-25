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
using CloneEbay.Services;
using Microsoft.AspNetCore.Http;

namespace CloneEbay.Pages
{
    public class PaymentModel : PageModel
    {
        private readonly CloneEbayDbContext _db;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly OrderProcessingService _orderProcessingService;
        private readonly IOrderTimeoutService _orderTimeoutService;
        public string StripePublishableKey { get; set; }
        public int OrderTimeoutSeconds { get; set; } = 60;
        public int SecondsLeft { get; set; } = 60;
        public PaymentModel(CloneEbayDbContext db, IPaymentService paymentService, IEmailService emailService, IConfiguration configuration, OrderProcessingService orderProcessingService, IOrderTimeoutService orderTimeoutService)
        {
            _db = db;
            _paymentService = paymentService;
            _emailService = emailService;
            _configuration = configuration;
            StripePublishableKey = _configuration["Stripe:PublishableKey"];
            _orderProcessingService = orderProcessingService;
            _orderTimeoutService = orderTimeoutService;
        }

        public OrderTable? Order { get; set; }
        public string? CouponCode { get; set; }
        public decimal? CouponDiscountPercent { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal? OrderTotal { get; set; }

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

            // Xác định key session cho order
            string sessionKey = $"PaymentStartTime_{orderId}";
            DateTime paymentStartTime;
            if (HttpContext.Session.GetString(sessionKey) == null)
            {
                paymentStartTime = DateTime.Now;
                HttpContext.Session.SetString(sessionKey, paymentStartTime.ToString("O"));
            }
            else
            {
                paymentStartTime = DateTime.Parse(HttpContext.Session.GetString(sessionKey));
            }
            var elapsed = (DateTime.Now - paymentStartTime).TotalSeconds;
            SecondsLeft = OrderTimeoutSeconds - (int)elapsed;
            if (SecondsLeft <= 0)
            {
                var cancelled = await _orderTimeoutService.CancelOrderIfTimeoutAsync(orderId, 0); // force cancel
                ErrorMessage = "Order has been cancelled due to payment timeout.";
                SecondsLeft = 0;
                return Page();
            }
            if (Order.Coupon != null)
            {
                CouponCode = Order.Coupon.Code;
                CouponDiscountPercent = Order.Coupon.DiscountPercent;
            }
            // --- TÍNH TỔNG TIỀN ĐƠN HÀNG ---
            string region = Order.Address != null ? Order.Address.GetRegion() : "Unknown";
            decimal productPrice = Order.OrderItems.Sum(oi => (oi.UnitPrice ?? 0) * (oi.Quantity ?? 0));
            int quantity = Order.OrderItems.Sum(oi => oi.Quantity ?? 0);
            decimal discount = (Order.Coupon?.DiscountPercent ?? 0) > 0 ? productPrice * (Order.Coupon.DiscountPercent ?? 0) / 100 : 0;
            var totalModel = await _orderProcessingService.CalculateOrderTotalAsync(productPrice, 1, region, discount);
            ShippingFee = totalModel.ShippingFee;
            Subtotal = totalModel.Subtotal;
            Discount = totalModel.Discount;
            OrderTotal = totalModel.Total;
            return Page();
        }

        public async Task<IActionResult> OnPostPayAsync(int orderId, string stripeToken)
        {
            var order = await _db.OrderTables
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Address)
                .Include(o => o.Buyer)
                .Include(o => o.ShippingInfos)
                .Include(o => o.Coupon)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return RedirectToPage("/Error");

            // Tính lại tổng tiền (bao gồm phí ship, giảm giá...)
            string region = order.Address != null ? order.Address.GetRegion() : "Unknown";
            decimal subtotal = order.OrderItems.Sum(oi => (oi.UnitPrice ?? 0) * (oi.Quantity ?? 0));
            decimal shippingFee = CloneEbay.Services.ShippingFeeCalculator.Calculate(region);
            decimal discount = (order.Coupon?.DiscountPercent ?? 0) > 0 ? subtotal * (order.Coupon.DiscountPercent ?? 0) / 100 : 0;
            decimal total = subtotal + shippingFee - discount;
            order.TotalPrice = total > 0 ? total : 0;
            await _db.SaveChangesAsync();

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

            // Nếu chưa có shipment cho order, tạo shipment mới
            if (order.ShippingInfos == null || !order.ShippingInfos.Any())
            {
                var shipment = new ShippingInfo
                {
                    OrderId = order.Id,
                    Carrier = "SimulatedCarrier",
                    TrackingNumber = $"SHIP-{order.Id}-{DateTime.Now.Ticks % 10000}",
                    Status = "InTransit",
                    EstimatedArrival = DateTime.Now.AddDays(3)
                };
                _db.ShippingInfos.Add(shipment);
                await _db.SaveChangesAsync();
            }

            // Send email to user (get from Buyer)
            var userEmail = order.Buyer?.Email ?? "";
            var subject = $"Order #{order.Id} Payment Confirmation";
            // Lấy thông tin shipment
            var shipmentForEmail = order.ShippingInfos?.FirstOrDefault();
            string shippingInfo = shipmentForEmail != null ? $@"
                <p><b>Shipping Carrier:</b> {shipmentForEmail.Carrier}</p>
                <p><b>Tracking Number:</b> {shipmentForEmail.TrackingNumber}</p>
                <p><b>Estimated Arrival:</b> {shipmentForEmail.EstimatedArrival?.ToString("dd/MM/yyyy")}</p>" : "<p><b>Shipping:</b> Not available</p>";
            // Coupon info
            string couponInfo = order.Coupon != null ? $"<p><b>Coupon:</b> {order.Coupon.Code} ({order.Coupon.DiscountPercent}% off)</p>" : "";
            // Shipping fee
            string regionForEmail = order.Address != null ? order.Address.GetRegion() : "Unknown";
            decimal shippingFeeForEmail = CloneEbay.Services.ShippingFeeCalculator.Calculate(regionForEmail);
            // Subtotal
            decimal subtotalForEmail = order.OrderItems.Sum(oi => (oi.UnitPrice ?? 0) * (oi.Quantity ?? 0));
            // Discount
            decimal discountForEmail = (order.Coupon?.DiscountPercent ?? 0) > 0 ? subtotalForEmail * (order.Coupon.DiscountPercent ?? 0) / 100 : 0;
            // Email body
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border:1px solid #eee; border-radius:8px; overflow:hidden;'>
                    <div style='background: #0064d2; color: #fff; padding: 24px 16px; text-align: center;'>
                        <h1 style='margin:0; font-size:40px; font-weight:bold; color:#fff; letter-spacing:2px;'>ebay</h1>
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
                        <div style='margin-bottom:8px;'><b>Subtotal:</b> ${subtotalForEmail.ToString("F2")}</div>
                        <div style='margin-bottom:8px;'><b>Shipping Fee:</b> ${shippingFeeForEmail.ToString("F2")}</div>
                        {couponInfo}
                        <div style='margin-bottom:8px;'><b>Discount:</b> -${discountForEmail.ToString("F2")}</div>
                        <div style='margin-bottom:16px;'><b>Total Paid:</b> <span style='color:#0064d2;'>${order.TotalPrice?.ToString("F2")}</span></div>
                        {shippingInfo}
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