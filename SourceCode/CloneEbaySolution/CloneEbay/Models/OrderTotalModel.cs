namespace CloneEbay.Models
{
    public class OrderTotalModel
    {
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
} 