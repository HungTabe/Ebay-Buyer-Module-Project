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
    }
} 