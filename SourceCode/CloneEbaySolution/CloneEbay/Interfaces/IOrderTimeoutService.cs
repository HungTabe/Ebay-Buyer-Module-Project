using System.Threading.Tasks;
namespace CloneEbay.Interfaces
{
    public interface IOrderTimeoutService
    {
        Task<bool> CancelOrderIfTimeoutAsync(int orderId, int timeoutSeconds);
    }
} 