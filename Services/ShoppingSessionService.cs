using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Extensions;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class ShoppingSessionService : IShoppingSessionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingSessionService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task DeleteFromHttpContext(HttpContext context)
        {
            var shoppingSession = await GetFromHttpContext(context);

            if (shoppingSession != null)
            {
                _context.ShoppingSessions.Remove(shoppingSession);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrderLineItem>> ExtractToOrderLineItems(Guid id)
        {
            var shoppingSession = await Get(id);

            return shoppingSession.CartItems.Select(item => new OrderLineItem
            {
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                UnitTaxes = item.UnitTaxes
            });
        }

        public async Task<ShoppingSession> Get(Guid id)
        {
            var shoppingSession = _context.ShoppingSessions
                .Include(s => s.CartItems).ThenInclude(c => c.Product).ThenInclude(p => p.Brand)
                .Include(s => s.CartItems).ThenInclude(c => c.Product).ThenInclude(p => p.Category);

            return await shoppingSession.FirstAsync(s => s.Id == id);
        }

        public async Task<ShoppingSession> GetFromHttpContext(HttpContext context)
        {
            if (context.User.IsAuthenticated())
            {
                var user = await _userManager.GetUserAsync(context.User);

                var shoppingSession = await _context.ShoppingSessions.FirstAsync(s => s.User == user);

                return await Get(shoppingSession.Id);
            }

            var currentShoppingId = context.Session.GetShoppingSessionId();

            if (!currentShoppingId.HasValue) throw new Exception("Empty shopping session");

            return await Get(currentShoppingId.Value);
        }

        public async Task UpdateStripePaymentIntentId(Guid id, string paymentIntentId)
        {
            var shoppingSessionId = await Get(id);

            if (shoppingSessionId == null) throw new Exception("Empty shopping session");

            shoppingSessionId.CurrentStripePaymentIntentId = paymentIntentId;

            await _context.SaveChangesAsync();
        }
    }
}
