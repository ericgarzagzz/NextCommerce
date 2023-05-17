using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Controllers
{
    [Authorize]
    public class UserDashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IWishlistService _wishlistService;

        public UserDashboardController(IOrderService orderService, IWishlistService wishlistService)
        {
            _orderService = orderService;
            _wishlistService = wishlistService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Orders()
        {
            return View(await _orderService.GetFromClaimPrincipal(User));
        }

        public async Task<IActionResult> Wishlist()
        {
            return View(await _wishlistService.Get(User));
        }

        public IActionResult SavedAddress()
        {
            return View();
        }

        public IActionResult Payment()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Security()
        {
            return View();
        }
    }
}
