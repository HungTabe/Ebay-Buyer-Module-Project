namespace CloneEbay.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body (HTML allowed).</param>
        /// <returns>Task representing the async operation.</returns>
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendPaymentSuccessEmail(string userId, string orderId, string transactionId);
        Task SendPaymentFailedEmail(string userId, string orderId);
        Task SendShippingFailedEmail(string userId, string orderId);
        Task SendOrderStatusChangedEmail(string userId, string orderId, string status);
    }
} 