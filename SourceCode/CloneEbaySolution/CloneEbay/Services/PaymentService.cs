using CloneEbay.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace CloneEbay.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly string _secretKey;

        public PaymentService(IConfiguration configuration)
        {
            _secretKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<bool> ProcessPaymentAsync(long amount, string currency, string source, string description)
        {
            var options = new ChargeCreateOptions
            {
                Amount = amount,
                Currency = currency,
                Source = source,
                Description = description,
            };
            var service = new ChargeService();
            try
            {
                Charge charge = await service.CreateAsync(options);
                return charge.Status == "succeeded";
            }
            catch
            {
                return false;
            }
        }
    }
} 