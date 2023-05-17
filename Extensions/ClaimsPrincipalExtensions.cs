using System.Security.Claims;

namespace NextCommerce.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAuthenticated(this ClaimsPrincipal user)
        {
            return user.Identity != null && user.Identity.IsAuthenticated;
        }
    }
}
