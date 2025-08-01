namespace CloneEbay.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendPaymentSuccessEmail(string userId, string orderId, string transactionId);
        Task SendPaymentFailedEmail(string userId, string orderId);
        Task SendShippingFailedEmail(string userId, string orderId);
        Task SendOrderStatusChangedEmail(string userId, string orderId, string status);
    }
} 