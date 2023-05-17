using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;

namespace NextCommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(string productName)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images).ThenInclude(i => i.Image)
                .Include(p => p.Dimension)
                .Include(p => p.Specifications)
                .FirstOrDefaultAsync(p => p.Name == productName);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
