using System;
using System.Threading.Tasks;
using CloneEbay.Interfaces;
using CloneEbay.Models;

namespace CloneEbay.Services
{
    public class OrderProcessingService
    {
        private readonly IPaymentProvider _paypalProvider;
        private readonly IPaymentProvider _codProvider;
        private readonly IShippingProvider _shippingProvider;
        private readonly IEmailService _emailService;

        public OrderProcessingService(IPaymentProvider paypalProvider, IPaymentProvider codProvider, IShippingProvider shippingProvider, IEmailService emailService)
        {
            _paypalProvider = paypalProvider;
            _codProvider = codProvider;
            _shippingProvider = shippingProvider;
            _emailService = emailService;
        }

        public async Task<OrderTotalModel> CalculateOrderTotalAsync(decimal productPrice, int quantity, string region, decimal discount)
        {
            var subtotal = productPrice * quantity;
            var shippingFee = ShippingFeeCalculator.Calculate(region);
            var total = subtotal + shippingFee - discount;
            return new OrderTotalModel
            {
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                Discount = discount,
                Total = total > 0 ? total : 0
            };
        }

        public async Task<bool> ProcessOrderAsync(PaymentRequestModel paymentRequest, ShippingRequestModel shippingRequest, decimal productPrice, int quantity, decimal discount)
        {
            // 1. Calculate total
            var orderTotal = await CalculateOrderTotalAsync(productPrice, quantity, shippingRequest.Region, discount);
            paymentRequest.Amount = orderTotal.Total;

            // 2. Select payment provider
            IPaymentProvider paymentProvider = paymentRequest.PaymentMethod == "PayPal" ? _paypalProvider : _codProvider;

            // 3. Process payment
            var paymentResult = await paymentProvider.ProcessPaymentAsync(paymentRequest);
            await TransactionLogger.LogAsync($"Order {paymentRequest.OrderId} - Payment: {paymentResult.TransactionId}, Success: {paymentResult.Success}, Message: {paymentResult.Message}");

            if (!paymentResult.Success)
            {
                await _emailService.SendPaymentFailedEmail(paymentRequest.UserId, paymentRequest.OrderId);
                return false;
            }

            // 4. Create shipment
            var shippingResult = await _shippingProvider.CreateShipmentAsync(shippingRequest);
            await TransactionLogger.LogAsync($"Order {paymentRequest.OrderId} - Shipment: {shippingResult.ShipmentCode}, Success: {shippingResult.Success}, Message: {shippingResult.Message}");

            if (!shippingResult.Success)
            {
                await _emailService.SendShippingFailedEmail(paymentRequest.UserId, paymentRequest.OrderId);
                return false;
            }

            // 5. Send payment success email
            await _emailService.SendPaymentSuccessEmail(paymentRequest.UserId, paymentRequest.OrderId, paymentResult.TransactionId);

            // 6. Simulate order status change and send email
            await _emailService.SendOrderStatusChangedEmail(paymentRequest.UserId, paymentRequest.OrderId, "Delivered");

            return true;
        }

        // Simulate auto-cancel: call this periodically or on order query
        public async Task<bool> AutoCancelOrderIfTimeoutAsync(string orderId, DateTime orderCreated, int timeoutMinutes)
        {
            if ((DateTime.Now - orderCreated).TotalMinutes > timeoutMinutes)
            {
                await TransactionLogger.LogAsync($"Order {orderId} auto-cancelled due to timeout.");
                // Send email notification
                await _emailService.SendOrderStatusChangedEmail("", orderId, "Cancelled");
                return true;
            }
            return false;
        }
    }
} 