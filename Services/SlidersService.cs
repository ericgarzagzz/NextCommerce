using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class SlidersService : ISlidersService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SlidersService> _logger;

        public SlidersService(ApplicationDbContext context, ILogger<SlidersService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<Product>> GetProductSlidersAsync()
        {
            return _context.Products
                .Include(p => p.SliderImage)
                .Include(p => p.Brand)
                .Include(p => p.Images).ThenInclude(i => i.Image)
                .OrderBy(p => p.SliderOrder)
                .Where(p => p.ShowInSlider).ToListAsync();
        }
    }
}
