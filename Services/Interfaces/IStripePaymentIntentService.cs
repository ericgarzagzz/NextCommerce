using Stripe;

namespace NextCommerce.Services.Interfaces
{
    public interface IStripePaymentIntentService
    {
        Task<PaymentIntent> Get(string paymentIntentId);
        Task<PaymentIntent> CreateFromHttpContextShoppingSession(HttpContext context);
        Task Cancel(string paymentIntentId);
    }
}
