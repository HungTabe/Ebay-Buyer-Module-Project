using System;
using System.Threading.Tasks;
using CloneEbay.Data;
using CloneEbay.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloneEbay.Services
{
    public class OrderTimeoutService : IOrderTimeoutService
    {
        private readonly CloneEbayDbContext _db;
        public OrderTimeoutService(CloneEbayDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CancelOrderIfTimeoutAsync(int orderId, int timeoutSeconds)
        {
            var order = await _db.OrderTables.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null || order.Status == "Cancelled" || order.Status == "Paid")
                return false;
            if (order.OrderDate.HasValue && (DateTime.Now - order.OrderDate.Value).TotalSeconds > timeoutSeconds)
            {
                order.Status = "Cancelled";
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
} 