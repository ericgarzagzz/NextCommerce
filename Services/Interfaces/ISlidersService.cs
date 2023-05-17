using NextCommerce.Data.Entities;

namespace NextCommerce.Services.Interfaces
{
    public interface ISlidersService
    {
        Task<List<Product>> GetProductSlidersAsync();
    }
}
