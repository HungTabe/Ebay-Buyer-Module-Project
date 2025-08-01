using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CloneEbay.Data;
using CloneEbay.Data.Entities;
using CloneEbay.Interfaces;

namespace CloneEbay.Services
{
    public class ShippingInfoService : IShippingInfoService
    {
        private readonly CloneEbayDbContext _db;
        public ShippingInfoService(CloneEbayDbContext db)
        {
            _db = db;
        }

        public async Task<List<ShippingInfo>> GetUserShipmentsAsync(int userId)
        {
            return await _db.ShippingInfos
                .Include(s => s.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(s => s.Order)
                    .ThenInclude(o => o.Address)
                .Where(s => s.Order != null && s.Order.BuyerId == userId)
                .ToListAsync();
        }

        public async Task<List<ShippingInfo>> SearchUserShipmentsAsync(int userId, string? orderId, string? trackingNumber, string? status, string? estimatedArrival)
        {
            var query = _db.ShippingInfos
                .Include(s => s.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(s => s.Order)
                    .ThenInclude(o => o.Address)
                .Where(s => s.Order != null && s.Order.BuyerId == userId);

            if (!string.IsNullOrWhiteSpace(orderId) && int.TryParse(orderId, out int oid))
                query = query.Where(s => s.OrderId == oid);
            if (!string.IsNullOrWhiteSpace(trackingNumber))
                query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(s => s.Status == status);
            if (!string.IsNullOrWhiteSpace(estimatedArrival) && System.DateTime.TryParse(estimatedArrival, out var estDate))
                query = query.Where(s => s.EstimatedArrival.HasValue && s.EstimatedArrival.Value.Date == estDate.Date);

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateShipmentStatusAsync(int shipmentId, int userId, string newStatus)
        {
            var shipment = await _db.ShippingInfos.Include(s => s.Order).FirstOrDefaultAsync(s => s.Id == shipmentId && s.Order.BuyerId == userId);
            if (shipment != null && !string.IsNullOrEmpty(newStatus))
            {
                shipment.Status = newStatus;
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
} 