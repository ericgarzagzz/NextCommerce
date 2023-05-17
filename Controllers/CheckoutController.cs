using FluentEmail.Core.Models;
using Hangfire;
using HashidsNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextCommerce.Data.Entities;
using NextCommerce.Extensions;
using NextCommerce.Models;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IShoppingSessionService _shoppingSessionService;
        private readonly IStripePaymentIntentService _stripePaymentIntentService;
        private readonly IOrderService _orderService;
        private readonly Hashids _hashIds;
        private readonly IEmailService _emailService;
        private readonly IEmailViewModelService _emailViewModelService;
        private readonly IBackgroundJobClient _backgroundJob;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IShoppingSessionService shoppingSessionService, IStripePaymentIntentService stripePaymentIntentService, IOrderService orderService, Hashids hashIds, IEmailService emailService, IEmailViewModelService emailViewModelService, IBackgroundJobClient backgroundJob, ILogger<CheckoutController> logger)
        {
            _shoppingSessionService = shoppingSessionService;
            _stripePaymentIntentService = stripePaymentIntentService;
            _orderService = orderService;
            _hashIds = hashIds;
            _emailService = emailService;
            _emailViewModelService = emailViewModelService;
            _backgroundJob = backgroundJob;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var shoppingSession = await _shoppingSessionService.GetFromHttpContext(HttpContext);

                if (!string.IsNullOrEmpty(shoppingSession.CurrentStripePaymentIntentId))
                    await _stripePaymentIntentService.Cancel(shoppingSession.CurrentStripePaymentIntentId);

                var paymentIntent = await _stripePaymentIntentService.CreateFromHttpContextShoppingSession(HttpContext);

                await _shoppingSessionService.UpdateStripePaymentIntentId(shoppingSession.Id, paymentIntent.Id);

                return View(new CheckoutViewModel(shoppingSession, paymentIntent.ClientSecret));
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error while attempting to load checkout page");

                return RedirectToActionPermanent("Index", "Home");
            }
        }

        public async Task<IActionResult> ProcessSuccessPayment(string payment_intent)
        {
            var paymentIntent = await _stripePaymentIntentService.Get(payment_intent);
            var shoppingSession = await _shoppingSessionService.GetFromHttpContext(HttpContext);

            var order = await _orderService.Create(paymentIntent, shoppingSession.Id);

            await _shoppingSessionService.DeleteFromHttpContext(HttpContext);

            var encodedOrderId = _hashIds.Encode(order.Id);

            var customerAddress = new Address(order.Details.User != null ? order.Details.User.Email : order.Details.Email);

            if (order.Details.Status == Data.Enums.OrderStatus.Pending)
                _backgroundJob.Enqueue(() => _emailService.SendOrderReceived(_emailViewModelService.GetOrderReceivedViewModel(order.Id), new[] { customerAddress }));
            else
                _backgroundJob.Enqueue(() => _emailService.SendOrderReceivedAndPaid(_emailViewModelService.GetOrderReceivedAndPaidViewModel(order.Id), new[] { customerAddress }));

            return RedirectToAction("OrderSuccess", new { hash = encodedOrderId });
        }

        public async Task<IActionResult> OrderSuccess(string hash)
        {
            if (!_hashIds.TryDecodeSingle(hash, out _)) return NotFound();

            var orderId = _hashIds.DecodeSingle(hash);

            var order = await _orderService.Get(orderId);

            if (order.Details.StripePaymentIntentStatus == "requires_action")
            {
                var paymentIntent = await _stripePaymentIntentService.Get(order.Details.StripePaymentIntentId);

                ViewBag.NextAction = paymentIntent.NextAction;
            }

            return View(order);
        }
    }
}
