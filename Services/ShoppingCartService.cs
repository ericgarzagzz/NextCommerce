using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Models.API;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartViewModel> GetCartFromSession(Guid shoppingSessionId)
        {
            var session = await _context.ShoppingSessions
                .Include(s => s.CartItems).ThenInclude(i => i.Product).ThenInclude(p => p.Images).ThenInclude(i => i.Image)
                .Include(s => s.CartItems).ThenInclude(i => i.Product).ThenInclude(p => p.Category)
                .Include(s => s.CartItems).ThenInclude(i => i.Product).ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(s => s.Id == shoppingSessionId);

            if (session == null) return new CartViewModel();

            return new CartViewModel
            {
                Total = session.Total,
                Taxes = session.Taxes,
                Fees = session.Fees,
                Items = session.CartItems.Select(i => new CartItemViewModel
                {
                    Id = i.Id,
                    Quantity = i.Quantity,
                    ProductName = i.Product.Name,
                    UnitPrice = i.UnitPrice,
                    UnitTaxes = i.UnitTaxes,
                    ProductImage = i.Product.Images.FirstOrDefault()?.Image.Path,
                    CategoryName = i.Product.Category?.Name,
                    BrandName = i.Product.Brand?.Name
                })
            };
        }

        public async Task<CartViewModel> GetCartFromUser(Guid userId)
        {
            var session = await _context.ShoppingSessions.FirstOrDefaultAsync(s => s.UserId == userId);

            if (session == null) return new CartViewModel();

            return await GetCartFromSession(session.Id);
        }
    }
}
