using Microsoft.AspNetCore.Identity;

namespace NextCommerce.Data.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? StripeCustomerId { get; set; }
    }
}
