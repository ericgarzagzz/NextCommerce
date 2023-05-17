using NextCommerce.Data.Entities;
using Stripe;
using System.Security.Claims;

namespace NextCommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> Create(PaymentIntent paymentIntent, Guid shoppingSessionId);
        Task<Order> Get(int id);
        Task<IEnumerable<Order>> GetFromClaimPrincipal(ClaimsPrincipal claim);
    }
}
