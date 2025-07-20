using CloneEbay.Models;

namespace CloneEbay.Interfaces
{
    public interface IOrderService
    {
        Task<OrderHistoryViewModel> GetOrderHistoryAsync(int userId, int page = 1, int pageSize = 10);
        Task<OrderDetailViewModel?> GetOrderDetailAsync(int orderId, int userId);
        Task<List<OrderStatusViewModel>> GetOrderStatusesAsync();
    }
} 