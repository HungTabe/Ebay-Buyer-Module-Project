using CloneEbay.Data;
using CloneEbay.Data.Entities;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(o => o.Coupon)
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
                    : "No address",
                CouponCode = o.Coupon?.Code
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

        public async Task<bool> CreateReturnRequestAsync(CreateReturnRequestModel model, int userId)
        {
            // Check if order exists and belongs to user
            var order = await _context.OrderTables
                .FirstOrDefaultAsync(o => o.Id == model.OrderId && o.BuyerId == userId);

            if (order == null)
                return false;

            // Check if order can be returned (delivered and within return window)
            if (!await CanReturnOrderAsync(model.OrderId, userId))
                return false;

            // Check if return request already exists
            var existingRequest = await _context.ReturnRequests
                .FirstOrDefaultAsync(r => r.OrderId == model.OrderId && r.UserId == userId);

            if (existingRequest != null)
                return false;

            var returnRequest = new ReturnRequest
            {
                OrderId = model.OrderId,
                UserId = userId,
                Reason = model.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.ReturnRequests.Add(returnRequest);
            await _context.SaveChangesAsync();

            // Cập nhật trạng thái đơn hàng
            order.Status = "Return Processing";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ReturnRequestListViewModel> GetReturnRequestsAsync(int userId, int page = 1, int pageSize = 10)
        {
            var query = _context.ReturnRequests
                .Include(r => r.Order)
                    .ThenInclude(o => o.Address)
                .Include(r => r.Order)
                    .ThenInclude(o => o.Coupon)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var returnRequests = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var returnRequestViewModels = returnRequests.Select(r => new ReturnRequestViewModel
            {
                Id = r.Id,
                OrderId = r.OrderId ?? 0,
                UserId = r.UserId ?? 0,
                Reason = r.Reason ?? "",
                Status = r.Status ?? "",
                CreatedAt = r.CreatedAt ?? DateTime.UtcNow,
                Order = r.Order != null ? new OrderSummaryViewModel
                {
                    Id = r.Order.Id,
                    OrderDate = r.Order.OrderDate ?? DateTime.UtcNow,
                    TotalPrice = r.Order.TotalPrice ?? 0,
                    Status = r.Order.Status ?? "",
                    ItemCount = r.Order.OrderItems?.Count ?? 0,
                    AddressInfo = r.Order.Address != null 
                        ? $"{r.Order.Address.FullName ?? ""} - {r.Order.Address.City ?? ""}, {r.Order.Address.State ?? ""}"
                        : "No address",
                    CouponCode = r.Order.Coupon?.Code
                } : null
            }).ToList();

            return new ReturnRequestListViewModel
            {
                ReturnRequests = returnRequestViewModels,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<ReturnRequestViewModel?> GetReturnRequestAsync(int returnRequestId, int userId)
        {
            var returnRequest = await _context.ReturnRequests
                .Include(r => r.Order)
                    .ThenInclude(o => o.Address)
                .Include(r => r.Order)
                    .ThenInclude(o => o.Coupon)
                .FirstOrDefaultAsync(r => r.Id == returnRequestId && r.UserId == userId);

            if (returnRequest == null)
                return null;

            return new ReturnRequestViewModel
            {
                Id = returnRequest.Id,
                OrderId = returnRequest.OrderId ?? 0,
                UserId = returnRequest.UserId ?? 0,
                Reason = returnRequest.Reason ?? "",
                Status = returnRequest.Status ?? "",
                CreatedAt = returnRequest.CreatedAt ?? DateTime.UtcNow,
                Order = returnRequest.Order != null ? new OrderSummaryViewModel
                {
                    Id = returnRequest.Order.Id,
                    OrderDate = returnRequest.Order.OrderDate ?? DateTime.UtcNow,
                    TotalPrice = returnRequest.Order.TotalPrice ?? 0,
                    Status = returnRequest.Order.Status ?? "",
                    ItemCount = returnRequest.Order.OrderItems?.Count ?? 0,
                    AddressInfo = returnRequest.Order.Address != null 
                        ? $"{returnRequest.Order.Address.FullName ?? ""} - {returnRequest.Order.Address.City ?? ""}, {returnRequest.Order.Address.State ?? ""}"
                        : "No address",
                    CouponCode = returnRequest.Order.Coupon?.Code
                } : null
            };
        }

        public async Task<bool> ConfirmOrderDeliveredAsync(int orderId, int userId)
        {
            var order = await _context.OrderTables.FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);
            if (order == null)
                return false;
            order.Status = "Delivered";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanReturnOrderAsync(int orderId, int userId)
        {
            var order = await _context.OrderTables
                .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);

            if (order == null)
                return false;

            // Check if order is delivered
            if (order.Status != "Delivered")
                return false;

            // Check if order is within return window (30 days from delivery)
            var orderDate = order.OrderDate ?? DateTime.UtcNow;
            var returnDeadline = orderDate.AddDays(30);
            
            if (DateTime.UtcNow > returnDeadline)
                return false;

            // Check if return request already exists
            var existingRequest = await _context.ReturnRequests
                .FirstOrDefaultAsync(r => r.OrderId == orderId && r.UserId == userId);
            return existingRequest == null;

            

        }

        public async Task<CouponValidationResult> ValidateAndApplyCouponAsync(string couponCode, List<CartItem> cartItems)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Please enter a coupon code." };
            }

            var now = DateTime.UtcNow;
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);
            if (coupon == null)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Coupon code not found." };
            }
            if (coupon.StartDate.HasValue && coupon.StartDate > now)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Coupon is not yet valid." };
            }
            if (coupon.EndDate.HasValue && coupon.EndDate < now)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Coupon has expired." };
            }
            if (coupon.MaxUsage.HasValue && coupon.UsedCount.HasValue && coupon.UsedCount >= coupon.MaxUsage)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Coupon usage limit reached." };
            }
            // If coupon is for a specific product, check if it's in the cart
            if (coupon.ProductId.HasValue && !cartItems.Any(ci => ci.ProductId == coupon.ProductId))
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Coupon is not applicable to your cart items." };
            }
            return new CouponValidationResult
            {
                IsValid = true,
                DiscountPercent = coupon.DiscountPercent ?? 0,
                CouponId = coupon.Id,
                ErrorMessage = null
            };
        }

        public async Task<OrderTable?> GetOrderWithCouponAsync(int orderId, int userId)
        {
            return await _context.OrderTables
                .Include(o => o.Coupon)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == userId);
        }

        public async Task<OrderTable?> CreateOrderAsync(int userId, int addressId, string couponCode, List<CartItem> cartItems)
        {
            try
            {
                // Validate address belongs to user
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
                
                if (address == null)
                    return null;

                // Calculate total price and apply coupon
                var totalPrice = cartItems.Sum(x => x.Price * x.Quantity);
                var discountedTotal = totalPrice;
                int? couponId = null;

                if (!string.IsNullOrWhiteSpace(couponCode))
                {
                    var couponResult = await ValidateAndApplyCouponAsync(couponCode, cartItems);
                    if (couponResult.IsValid)
                    {
                        discountedTotal = totalPrice * (1 - (couponResult.DiscountPercent / 100));
                        couponId = couponResult.CouponId;
                    }
                    else
                    {
                        // Return null if coupon is invalid
                        return null;
                    }
                }

                // Create order
                var order = new OrderTable
                {
                    BuyerId = userId,
                    AddressId = addressId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = discountedTotal,
                    Status = "Pending",
                    CouponId = couponId
                };

                _context.OrderTables.Add(order);
                await _context.SaveChangesAsync();

                // Create order items
                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    };
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                return order;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateCouponUsageAsync(int couponId)
        {
            try
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);
                if (coupon != null)
                {
                    coupon.UsedCount = (coupon.UsedCount ?? 0) + 1;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
} 