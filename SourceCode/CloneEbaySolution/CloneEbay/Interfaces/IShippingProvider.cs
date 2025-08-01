using System.Threading.Tasks;
using CloneEbay.Models;

namespace CloneEbay.Interfaces
{
    public interface IShippingProvider
    {
        Task<ShippingResultModel> CreateShipmentAsync(ShippingRequestModel request);
        Task<ShippingStatusModel> UpdateShipmentStatusAsync(string shipmentCode, string status);
        string Name { get; }
    }
} 