using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Models;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriesService> _logger;

        public CategoriesService(ApplicationDbContext context, ILogger<CategoriesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CategoryMenuViewModel> GetMenuViewModel()
        {
            var categories = await _context.Categories.ToListAsync();

            var viewModel = new CategoryMenuViewModel();
            viewModel.Items = new List<CategoryMenuItem>();

            foreach (var category in categories.Where(x => x.Parent == null))
            {
                viewModel.Items.Add(await GetMenuItem(category, categories));
            }

            return viewModel;
        }

        public Task<List<Category>> GetNewCategories()
        {
            return _context.Categories.Where(c => c.IsNew).ToListAsync();
        }

        private async Task<CategoryMenuItem> GetMenuItem(Category category, List<Category> categories)
        {
            var item = new CategoryMenuItem
            {
                Id = category.Id,
                Name = category.Name,
                Children = new List<CategoryMenuItem>()
            };

            foreach (var childCategory in categories.Where(x => x.Parent?.Id == category.Id))
            {
                item.Children.Add(await GetMenuItem(childCategory, categories));
            }

            return item;
        }
    }
}
