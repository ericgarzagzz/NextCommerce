using NextCommerce.Extensions;
using NextCommerce.Services.Interfaces;
using Stripe;

namespace NextCommerce.Services
{
    public class StripePaymentIntentService : IStripePaymentIntentService
    {
        private readonly IShoppingSessionService _shoppingSessionService;
        private readonly IStripeClientProviderService _stripeClientProviderService;

        public StripePaymentIntentService(IShoppingSessionService shoppingSessionService, IStripeClientProviderService stripeClientProviderService)
        {
            _shoppingSessionService = shoppingSessionService;
            _stripeClientProviderService = stripeClientProviderService;
        }

        public async Task Cancel(string paymentIntentId)
        {
            var stripeClient = await _stripeClientProviderService.GetClient();

            var paymentIntentService = new PaymentIntentService(stripeClient);

            paymentIntentService.Cancel(paymentIntentId);
        }

        public async Task<PaymentIntent> CreateFromHttpContextShoppingSession(HttpContext context)
        {
            var shoppingSession = await _shoppingSessionService.GetFromHttpContext(context);

            if (shoppingSession == null) throw new Exception("El carrito està vacío");

            var stripeClient = await _stripeClientProviderService.GetClient();

            var paymentIntentService = new PaymentIntentService(stripeClient);
            var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = shoppingSession.Total.ToCents(),
                Currency = "mxn",
                PaymentMethodTypes = new List<string> { "card", "oxxo" }
            });

            return paymentIntent;
        }

        public async Task<PaymentIntent> Get(string paymentIntentId)
        {
            var stripeClient = await _stripeClientProviderService.GetClient();

            var paymentIntentService = new PaymentIntentService(stripeClient);

            return await paymentIntentService.GetAsync(paymentIntentId, new PaymentIntentGetOptions
            {
                Expand = new List<string> { "latest_charge", "payment_method" }
            });
        }
    }
}
