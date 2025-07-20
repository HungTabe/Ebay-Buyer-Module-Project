namespace CloneEbay.Models
{
    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public decimal DiscountPercent { get; set; }
        public int? CouponId { get; set; }
        public string? ErrorMessage { get; set; }
    }
} 