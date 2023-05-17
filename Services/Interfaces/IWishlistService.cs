using NextCommerce.Data.Entities;
using System.Security.Claims;

namespace NextCommerce.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishItem>> Get(ClaimsPrincipal principal);
        Task<bool> Any(ClaimsPrincipal principal);
    }
}
