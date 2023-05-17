using NextCommerce.Data.Entities;

namespace NextCommerce.Services.Interfaces
{
    public interface IShoppingSessionService
    {
        Task<ShoppingSession> Get(Guid id);
        Task<ShoppingSession> GetFromHttpContext(HttpContext context);
        Task DeleteFromHttpContext(HttpContext context);
        Task UpdateStripePaymentIntentId(Guid id, string paymentIntentId);
        Task<IEnumerable<OrderLineItem>> ExtractToOrderLineItems(Guid id);
    }
}
