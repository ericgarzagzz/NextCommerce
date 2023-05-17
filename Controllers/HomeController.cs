using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Models;
using System.Diagnostics;

namespace NextCommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ILogger<HomeController> logger)
        {
            _context = context;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 60 * 60 * 24, Location = ResponseCacheLocation.Any)]
        public IActionResult GetAllProducts()
        {
            return Ok(_context.Products.Where(p => p.OnSale).Select(p => new ProductSearchItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price.ToString("C"),
                SalePrice = p.SalePrice.ToString("C"),
                Image = p.Images.Any() ? p.Images.First().Image.Path : null,
                ShowOldPrice = p.SalePrice < p.Price
            }).ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> InitDevelopment()
        {
            string username = _configuration.GetValue<string>("DefaultAdminUser:UserName");
            string password = _configuration.GetValue<string>("DefaultAdminUser:Password");

            var signInResult = await _signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            return Conflict(signInResult);
        }
    }
}