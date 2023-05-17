using NextCommerce.Data.Entities;

namespace NextCommerce.Services.Interfaces
{
    public interface IProductShowcaseCollectionService
    {
        Task<List<ProductShowcaseCollection>> GetAllAsync();
    }
}
