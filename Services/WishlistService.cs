using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Services.Interfaces;
using System.Security.Claims;

namespace NextCommerce.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> Any(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);

            return await _context.Wishlist.AnyAsync(i => i.User == user);
        }

        public async Task<IEnumerable<WishItem>> Get(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);

            return await _context.Wishlist
                .Include(i => i.Product).ThenInclude(i => i.Images).ThenInclude(i => i.Image)
                .Where(i => i.User == user).ToListAsync();
        }
    }
}
