using CloneEbay.Data.Entities;
using CloneEbay.Models;

namespace CloneEbay.Interfaces
{
    public interface IOrderService
    {
        Task<OrderHistoryViewModel> GetOrderHistoryAsync(int userId, int page = 1, int pageSize = 10);
        Task<OrderDetailViewModel?> GetOrderDetailAsync(int orderId, int userId);
        Task<List<OrderStatusViewModel>> GetOrderStatusesAsync();
        
        /// <summary>
        /// Tạo yêu cầu trả hàng cho đơn hàng. Nếu thành công, trạng thái đơn hàng sẽ chuyển thành 'Return Request Processing'.
        /// </summary>
        Task<bool> CreateReturnRequestAsync(CreateReturnRequestModel model, int userId);
        Task<ReturnRequestListViewModel> GetReturnRequestsAsync(int userId, int page = 1, int pageSize = 10);
        Task<ReturnRequestViewModel?> GetReturnRequestAsync(int returnRequestId, int userId);
        Task<bool> CanReturnOrderAsync(int orderId, int userId);
        Task<bool> ConfirmOrderDeliveredAsync(int orderId, int userId);
        /// <summary>
        /// Validates a coupon code and returns the discount percent if valid, otherwise 0.
        /// </summary>
        Task<CouponValidationResult> ValidateAndApplyCouponAsync(string couponCode, List<CartItem> cartItems);
        
        /// <summary>
        /// Gets order with coupon information for display purposes.
        /// </summary>
        Task<OrderTable?> GetOrderWithCouponAsync(int orderId, int userId);
        
        /// <summary>
        /// Creates a new order with items from cart and applies coupon if provided.
        /// </summary>
        Task<OrderTable?> CreateOrderAsync(int userId, int addressId, string couponCode, List<CartItem> cartItems);
        
        /// <summary>
        /// Updates coupon usage count after order is created.
        /// </summary>
        Task<bool> UpdateCouponUsageAsync(int couponId);
    }
} 