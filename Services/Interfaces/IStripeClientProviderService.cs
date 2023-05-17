using Stripe;

namespace NextCommerce.Services.Interfaces
{
    public interface IStripeClientProviderService
    {
        Task<IStripeClient> GetClient();
    }
}
