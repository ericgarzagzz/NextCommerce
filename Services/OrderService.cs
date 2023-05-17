using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Extensions;
using NextCommerce.Services.Interfaces;
using NuGet.Packaging;
using Stripe;
using System.Security.Claims;

namespace NextCommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingSessionService _shoppingSessionService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IShoppingSessionService shoppingSessionService, ILogger<OrderService> logger)
        {
            _context = context;
            _userManager = userManager;
            _shoppingSessionService = shoppingSessionService;
            _logger = logger;
        }

        public async Task<Order> Create(PaymentIntent paymentIntent, Guid shoppingSessionId)
        {
            var order = new Order();

            order.CreatedAt = DateTimeOffset.Now;
            order.Total = paymentIntent.Amount.ToMexicanPesos();

            var shoppingSession = await _shoppingSessionService.Get(shoppingSessionId);

            order.Fees = shoppingSession.Fees;

            order.Details = new OrderDetails
            {
                StripePaymentIntentId = paymentIntent.Id,
                StripePaymentIntentStatus = paymentIntent.Status,
                StripePaymentMethod = paymentIntent.PaymentMethod.Type,
                User = shoppingSession.User,
                Email = paymentIntent.ReceiptEmail,
                ShippingAddress = paymentIntent.Shipping.ToAddressInfo(),
                BillingAddress = paymentIntent.LatestCharge != null ? paymentIntent.LatestCharge.BillingDetails.ToAddressInfo() : new AddressInfo("", "", "", "", "", "", "", "", "", null),
                Status = paymentIntent.Status == "succeeded" ? Data.Enums.OrderStatus.PaymentReceived : Data.Enums.OrderStatus.Pending
            };

            var orderLineItems = await _shoppingSessionService.ExtractToOrderLineItems(shoppingSessionId);

            order.LineItems.AddRange(orderLineItems);

            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> Get(int id)
        {
            return await _context.Orders
                .Include(o => o.Details).ThenInclude(d => d.User)
                .Include(o => o.Details).ThenInclude(d => d.ShippingAddress)
                .Include(o => o.LineItems).ThenInclude(i => i.Product).ThenInclude(p => p.Images).ThenInclude(i => i.Image)
                .FirstAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetFromClaimPrincipal(ClaimsPrincipal claim)
        {
            var user = await _userManager.GetUserAsync(claim);

            return await _context.Orders
                .Include(o => o.Details)
                .Where(o => o.Details.User == user).ToListAsync();
        }
    }
}
