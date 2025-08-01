namespace CloneEbay.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(long amount, string currency, string source, string description);
    }
} 