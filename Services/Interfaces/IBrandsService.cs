using NextCommerce.Data.Entities;

namespace NextCommerce.Services.Interfaces
{
    public interface IBrandsService
    {
        Task<List<Brand>> GetPromotingBrandsAsync();
    }
}
