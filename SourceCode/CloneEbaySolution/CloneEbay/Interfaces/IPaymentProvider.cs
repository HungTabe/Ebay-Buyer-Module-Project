using System.Threading.Tasks;
using CloneEbay.Models;

namespace CloneEbay.Interfaces
{
    public interface IPaymentProvider
    {
        Task<PaymentResultModel> ProcessPaymentAsync(PaymentRequestModel request);
        Task<PaymentStatusModel> GetPaymentStatusAsync(string transactionId);
        string Name { get; }
    }
} 