namespace CloneEbay.Models
{
    public class ShippingStatusModel
    {
        public string ShipmentCode { get; set; }
        public string Status { get; set; } // Created, InTransit, Delivered, Failed
        public string Message { get; set; }
    }
} 