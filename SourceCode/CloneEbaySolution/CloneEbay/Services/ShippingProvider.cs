using System;
using System.Threading.Tasks;
using CloneEbay.Interfaces;
using CloneEbay.Models;

namespace CloneEbay.Services
{
    public class ShippingProvider : IShippingProvider
    {
        public string Name => "SimulatedShipping";

        public async Task<ShippingResultModel> CreateShipmentAsync(ShippingRequestModel request)
        {
            // Simulate shipment creation with retry
            int retry = 0;
            while (retry < 3)
            {
                try
                {
                    await Task.Delay(500);
                    if (new Random().Next(0, 10) < 2) // 20% fail
                        throw new Exception("Simulated API failure");
                    return new ShippingResultModel
                    {
                        ShipmentCode = Guid.NewGuid().ToString(),
                        Success = true,
                        Message = "Shipment created successfully."
                    };
                }
                catch
                {
                    retry++;
                    if (retry == 3)
                        return new ShippingResultModel
                        {
                            ShipmentCode = null,
                            Success = false,
                            Message = "Failed to create shipment after retries."
                        };
                }
            }
            return null;
        }

        public async Task<ShippingStatusModel> UpdateShipmentStatusAsync(string shipmentCode, string status)
        {
            await Task.Delay(200);
            return new ShippingStatusModel
            {
                ShipmentCode = shipmentCode,
                Status = status,
                Message = $"Shipment status updated to {status}."
            };
        }
    }
} 