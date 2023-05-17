using NextCommerce.Data.Entities;

namespace NextCommerce.Services.Interfaces
{
    public interface IServiceBannersService
    {
        Task<List<ServiceBanner>> GetAllAsync();
    }
}
