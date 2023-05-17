using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class ProductShowcaseCollectionService : IProductShowcaseCollectionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductShowcaseCollectionService> _logger;

        public ProductShowcaseCollectionService(ApplicationDbContext context, ILogger<ProductShowcaseCollectionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<ProductShowcaseCollection>> GetAllAsync()
        {
            return _context.ProductShowcaseCollections
                .Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Category)
                .Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Images).ThenInclude(i => i.Image)
                .Where(c => c.Items.Any())
                .OrderBy(c => c.Order)
                .ToListAsync();
        }
    }
}
