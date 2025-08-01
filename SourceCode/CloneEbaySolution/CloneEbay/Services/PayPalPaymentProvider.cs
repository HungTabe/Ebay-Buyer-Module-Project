using System.Threading.Tasks;
using CloneEbay.Interfaces;
using CloneEbay.Models;

namespace CloneEbay.Services
{
    public class PayPalPaymentProvider : IPaymentProvider
    {
        public string Name => "PayPal";

        public async Task<PaymentResultModel> ProcessPaymentAsync(PaymentRequestModel request)
        {
            // Simulate payment processing delay
            await Task.Delay(1000);
            return new PaymentResultModel
            {
                TransactionId = Guid.NewGuid().ToString(),
                Success = true,
                Message = "PayPal payment simulated successfully."
            };
        }

        public async Task<PaymentStatusModel> GetPaymentStatusAsync(string transactionId)
        {
            await Task.Delay(200);
            return new PaymentStatusModel
            {
                TransactionId = transactionId,
                Status = "Success",
                Message = "Payment completed."
            };
        }
    }
} 