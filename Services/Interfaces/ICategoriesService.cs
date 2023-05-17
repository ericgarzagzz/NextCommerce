using NextCommerce.Data.Entities;
using NextCommerce.Models;

namespace NextCommerce.Services.Interfaces
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetNewCategories();
        Task<CategoryMenuViewModel> GetMenuViewModel();
    }
}
