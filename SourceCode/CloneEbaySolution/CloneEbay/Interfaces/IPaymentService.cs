namespace CloneEbay.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Processes a payment using Stripe.
        /// </summary>
        /// <param name="amount">Amount in smallest currency unit (e.g., cents).</param>
        /// <param name="currency">Currency code (e.g., "usd").</param>
        /// <param name="source">Stripe token or payment method id.</param>
        /// <param name="description">Payment description.</param>
        /// <returns>True if payment succeeded, false otherwise.</returns>
        Task<bool> ProcessPaymentAsync(long amount, string currency, string source, string description);
    }
} 