using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Services.Interfaces;
using Stripe;

namespace NextCommerce.Services
{
    public class StripeClientProviderService : IStripeClientProviderService
    {
        private readonly ISettingsService _settingsService;

        public StripeClientProviderService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IStripeClient> GetClient()
        {
            var stripeSecretKey = await _settingsService.GetAsync(Data.Enums.SettingType.STRIPE_SECRET_KEY);

            if (string.IsNullOrEmpty(stripeSecretKey)) throw new Exception("No existe una configuración de pago actual registrada en el sistema.");

            return new StripeClient(stripeSecretKey);
        }
    }
}
