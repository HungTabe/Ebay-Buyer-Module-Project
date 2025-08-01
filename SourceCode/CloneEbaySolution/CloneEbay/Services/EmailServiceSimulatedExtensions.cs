using System.Threading.Tasks;

namespace CloneEbay.Services
{
    public partial class EmailService
    {
        public async Task SendPaymentSuccessEmail(string userId, string orderId, string transactionId)
        {
            await TransactionLogger.LogAsync($"Email: Payment success for Order {orderId}, User {userId}, Transaction {transactionId}");
        }
        public async Task SendPaymentFailedEmail(string userId, string orderId)
        {
            await TransactionLogger.LogAsync($"Email: Payment failed for Order {orderId}, User {userId}");
        }
        public async Task SendShippingFailedEmail(string userId, string orderId)
        {
            await TransactionLogger.LogAsync($"Email: Shipping failed for Order {orderId}, User {userId}");
        }
        public async Task SendOrderStatusChangedEmail(string userId, string orderId, string status)
        {
            await TransactionLogger.LogAsync($"Email: Order {orderId}, User {userId}, Status changed to {status}");
        }
    }
} 