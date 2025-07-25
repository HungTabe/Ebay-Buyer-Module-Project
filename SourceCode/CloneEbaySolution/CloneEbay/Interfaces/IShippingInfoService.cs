using System.Collections.Generic;
using System.Threading.Tasks;
using CloneEbay.Data.Entities;

namespace CloneEbay.Interfaces
{
    public interface IShippingInfoService
    {
        Task<List<ShippingInfo>> GetUserShipmentsAsync(int userId);
        Task<List<ShippingInfo>> SearchUserShipmentsAsync(int userId, string? orderId, string? trackingNumber, string? status, string? estimatedArrival);
        Task<bool> UpdateShipmentStatusAsync(int shipmentId, int userId, string newStatus);
    }
} 