using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class BrandsService : IBrandsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BrandsService> _logger;

        public BrandsService(ApplicationDbContext context, ILogger<BrandsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<Brand>> GetPromotingBrandsAsync()
        {
            return _context.Brands
                .Include(b => b.Logo)
                .Where(b => b.ShouldPromote).ToListAsync();
        }
    }
}
