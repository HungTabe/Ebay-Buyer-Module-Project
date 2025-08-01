namespace CloneEbay.Models
{
    public class PaymentStatusModel
    {
        public string TransactionId { get; set; }
        public string Status { get; set; } // Pending, Success, Failed
        public string Message { get; set; }
    }
} 