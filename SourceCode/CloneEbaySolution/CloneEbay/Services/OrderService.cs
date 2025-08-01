using CloneEbay.Data;
using CloneEbay.Data.Entities;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.EntityFrameworkCore;

namespace CloneEbay.Services
{
    public class OrderService : IOrderService
    {
        private readonly CloneEbayDbContext _context;

        public OrderService(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<OrderHistoryViewModel> GetOrderHistoryAsync(int userId, int page = 1, int pageSize = 10)
        {
            var query = _context.OrderTables
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .Where(o => o.BuyerId == userId)
                .OrderByDescending(o => o.OrderDate);

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Apply pagination
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Convert to view models
            var orderViewModels = orders.Select(o => new OrderSummaryViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate ?? DateTime.UtcNow,
                TotalPrice = o.TotalPrice ?? 0,
                Status = o.Status ?? "Unknown",
                ItemCount = o.OrderItems?.Count ?? 0,
                AddressInfo = o.Address != null 
                    ? $"{o.Address.FullName ?? ""} - {o.Address.City ?? ""}, {o.Address.State ?? ""}"
                    : "No address"
            }).ToList();

            return new OrderHistoryViewModel
            {
                Orders = orderViewModels,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderDetailViewModel?> GetOrderDetailAsync(int orderId, int userId)
        {
            var order = await _context.OrderTables
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(o => o.Payments)
                .Include(o => o.ShippingInfos)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);

            if (order == null)
                return null;

            var orderItems = order.OrderItems?.Select(oi => new OrderItemViewModel
            {
                Id = oi.Id,
                ProductId = oi.ProductId ?? 0,
                ProductName = oi.Product?.Title ?? "Unknown Product",
                ProductImage = oi.Product?.Images ?? "",
                Quantity = oi.Quantity ?? 0,
                UnitPrice = oi.UnitPrice ?? 0,
                TotalPrice = (oi.Quantity ?? 0) * (oi.UnitPrice ?? 0)
            }).ToList() ?? new List<OrderItemViewModel>();

            var address = order.Address != null ? new AddressViewModel
            {
                FullName = order.Address.FullName ?? "",
                Phone = order.Address.Phone ?? "",
                Street = order.Address.Street ?? "",
                City = order.Address.City ?? "",
                State = order.Address.State ?? "",
                Country = order.Address.Country ?? "",
                PostalCode = order.Address.PostalCode ?? ""
            } : new AddressViewModel();

            var payment = order.Payments?.FirstOrDefault();
            var paymentViewModel = payment != null ? new PaymentViewModel
            {
                Id = payment.Id,
                Amount = payment.Amount ?? 0,
                PaymentMethod = payment.Method ?? "",
                Status = payment.Status ?? "",
                PaymentDate = payment.PaidAt ?? DateTime.UtcNow
            } : null;

            var shippingInfo = order.ShippingInfos?.FirstOrDefault();
            var shippingViewModel = shippingInfo != null ? new ShippingInfoViewModel
            {
                Id = shippingInfo.Id,
                TrackingNumber = shippingInfo.TrackingNumber ?? "",
                Carrier = shippingInfo.Carrier ?? "",
                Status = shippingInfo.Status ?? "",
                ShippedDate = null, // ShippingInfo doesn't have ShippedDate property
                EstimatedDelivery = shippingInfo.EstimatedArrival
            } : null;

            return new OrderDetailViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate ?? DateTime.UtcNow,
                TotalPrice = order.TotalPrice ?? 0,
                Status = order.Status ?? "Unknown",
                Address = address,
                Items = orderItems,
                Payment = paymentViewModel,
                ShippingInfo = shippingViewModel
            };
        }

        public async Task<List<OrderStatusViewModel>> GetOrderStatusesAsync()
        {
            return new List<OrderStatusViewModel>
            {
                new OrderStatusViewModel { Value = "Pending", DisplayName = "Chờ xử lý" },
                new OrderStatusViewModel { Value = "Processing", DisplayName = "Đang xử lý" },
                new OrderStatusViewModel { Value = "Shipped", DisplayName = "Đã gửi hàng" },
                new OrderStatusViewModel { Value = "Delivered", DisplayName = "Đã giao hàng" },
                new OrderStatusViewModel { Value = "Cancelled", DisplayName = "Đã hủy" },
                new OrderStatusViewModel { Value = "Returned", DisplayName = "Đã trả hàng" }
            };
        }
    }
} 