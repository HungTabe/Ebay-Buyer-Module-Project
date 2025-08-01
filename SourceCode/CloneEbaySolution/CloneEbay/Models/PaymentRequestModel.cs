namespace CloneEbay.Models
{
    public class PaymentRequestModel
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // PayPal, COD
        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
} 