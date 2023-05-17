using NextCommerce.Models.API;

namespace NextCommerce.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<CartViewModel> GetCartFromUser(Guid userId);
        Task<CartViewModel> GetCartFromSession(Guid shoppingSessionId);
    }
}
