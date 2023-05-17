using HashidsNet;

namespace NextCommerce.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHashIds(this IServiceCollection services, string salt = "")
        {
            if (string.IsNullOrWhiteSpace(salt)) salt = Guid.NewGuid().ToString("n").Substring(0, 8);

            services.AddScoped(provider => new Hashids(salt, alphabet: "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", seps: "CFHISTU", minHashLength: 6));

            return services;
        }
    }
}
