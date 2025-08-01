using System;
using System.Collections.Generic;

namespace CloneEbay.Models
{
    public class OrderHistoryViewModel
    {
        public List<OrderSummaryViewModel> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "";
        public int ItemCount { get; set; }
        public string AddressInfo { get; set; } = "";
    }

    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "";
        public AddressViewModel Address { get; set; } = new();
        public List<OrderItemViewModel> Items { get; set; } = new();
        public PaymentViewModel? Payment { get; set; }
        public ShippingInfoViewModel? ShippingInfo { get; set; }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductImage { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class AddressViewModel
    {
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Country { get; set; } = "";
        public string PostalCode { get; set; } = "";
    }

    public class PaymentViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime PaymentDate { get; set; }
    }

    public class ShippingInfoViewModel
    {
        public int Id { get; set; }
        public string TrackingNumber { get; set; } = "";
        public string Carrier { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
    }

    public class OrderStatusViewModel
    {
        public string Value { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }
} 