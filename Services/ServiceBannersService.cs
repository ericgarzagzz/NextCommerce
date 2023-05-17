using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class ServiceBannersService : IServiceBannersService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceBannersService> _logger;

        public ServiceBannersService(ApplicationDbContext context, ILogger<ServiceBannersService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<ServiceBanner>> GetAllAsync()
        {
            return _context.ServiceBanners.OrderBy(banner => banner.Order).ToListAsync();
        }
    }
}
