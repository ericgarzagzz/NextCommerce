using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Extensions;
using NextCommerce.Models.API;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IShoppingCartService shoppingCartService, ILogger<CartController> logger)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var cart = new CartViewModel();

            if (User.IsAuthenticated())
            {
                var user = await _userManager.GetUserAsync(User);
                cart = await _shoppingCartService.GetCartFromUser(user.Id);
            }
            else
            {
                var shoppingSessionId = HttpContext.Session.GetShoppingSessionId();

                if (shoppingSessionId.HasValue)
                    cart = await _shoppingCartService.GetCartFromSession(shoppingSessionId.Value);
            }

            return View(cart);
        }

        public async Task<IActionResult> Remove(int id)
        {
            ShoppingSession? shoppingSession = null;

            var shoppingSessionQuery = _context.ShoppingSessions
                .Include(s => s.User)
                .Include(s => s.CartItems);

            if (User.IsAuthenticated())
            {
                var user = await _userManager.GetUserAsync(User);
                shoppingSession = await shoppingSessionQuery.FirstOrDefaultAsync(s => s.UserId == user.Id);
            } else
            {
                var shoppingSessionId = HttpContext.Session.GetShoppingSessionId();
                shoppingSession = await shoppingSessionQuery.FirstOrDefaultAsync(s => s.Id == shoppingSessionId && !s.UserId.HasValue);
            }

            var cartItem = shoppingSession?.CartItems.FirstOrDefault(i => i.Id == id);

            if (shoppingSession == null || cartItem == null) return NotFound();

            shoppingSession.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            shoppingSession.Total = shoppingSession.CartItems.Sum(c => c.UnitPrice * c.Quantity);
            shoppingSession.Taxes = shoppingSession.CartItems.Sum(c => c.UnitTaxes * c.Quantity);

            await _context.SaveChangesAsync();

            return RedirectToActionPermanent(nameof(Index));
        }

        public async Task<IActionResult> RemoveAll()
        {
            ShoppingSession? shoppingSession = null;

            var shoppingSessionQuery = _context.ShoppingSessions
                .Include(s => s.User)
                .Include(s => s.CartItems);

            if (User.IsAuthenticated())
            {
                var user = await _userManager.GetUserAsync(User);
                shoppingSession = await shoppingSessionQuery.FirstOrDefaultAsync(s => s.UserId == user.Id);
            }
            else
            {
                var shoppingSessionId = HttpContext.Session.GetShoppingSessionId();
                shoppingSession = await shoppingSessionQuery.FirstOrDefaultAsync(s => s.Id == shoppingSessionId && !s.UserId.HasValue);
            }

            if (shoppingSession == null) return NotFound();

            shoppingSession.CartItems.Clear();
            shoppingSession.Total = 0;
            shoppingSession.Taxes = 0;

            await _context.SaveChangesAsync();

            return RedirectToActionPermanent(nameof(Index));
        }
    }
}
