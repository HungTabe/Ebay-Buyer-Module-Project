namespace CloneEbay.Models
{
    public class PaymentResultModel
    {
        public string TransactionId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
} 