using System.Threading.Tasks;
using CloneEbay.Interfaces;
using CloneEbay.Models;

namespace CloneEbay.Services
{
    public class CodPaymentProvider : IPaymentProvider
    {
        public string Name => "COD";

        public async Task<PaymentResultModel> ProcessPaymentAsync(PaymentRequestModel request)
        {
            // Simulate COD approval
            await Task.Delay(500);
            return new PaymentResultModel
            {
                TransactionId = Guid.NewGuid().ToString(),
                Success = true,
                Message = "COD payment simulated successfully."
            };
        }

        public async Task<PaymentStatusModel> GetPaymentStatusAsync(string transactionId)
        {
            await Task.Delay(100);
            return new PaymentStatusModel
            {
                TransactionId = transactionId,
                Status = "Pending",
                Message = "COD payment pending."
            };
        }
    }
} 