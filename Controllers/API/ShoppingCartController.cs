using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;
using NextCommerce.Extensions;
using NextCommerce.Models.API;
using NextCommerce.Services;
using NextCommerce.Services.Interfaces;
using NextCommerce.Utils;

namespace NextCommerce.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStripePaymentIntentService _stripePaymentIntentService;
        private readonly ISettingsService _settingsService;
        private readonly ILogger<ShoppingCartController> _logger;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IShoppingCartService shoppingCartService, IStripePaymentIntentService stripePaymentIntentService, ISettingsService settingsService, ILogger<ShoppingCartController> logger)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _stripePaymentIntentService = stripePaymentIntentService;
            _settingsService = settingsService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CartViewModel>> Get()
        {
            var cart = new CartViewModel();

            if (User.IsAuthenticated())
            {
                var user = await _userManager.GetUserAsync(User);
                cart = await _shoppingCartService.GetCartFromUser(user.Id);
            } else
            {
                var shoppingSessionId = HttpContext.Session.GetShoppingSessionId();

                if (shoppingSessionId.HasValue)
                    cart = await _shoppingCartService.GetCartFromSession(shoppingSessionId.Value);
            }

            return Ok(cart);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartItemViewModel>> GetById(int id, CancellationToken cancellationToken)
        {
            var cartItemQuery = _context.CartItems
                    .Include(c => c.Product).ThenInclude(p => p.Category)
                    .Include(c => c.Product).ThenInclude(p => p.Brand)
                    .Include(c => c.Product).ThenInclude(p => p.Images).ThenInclude(i => i.Image)
                    .Include(c => c.Product).ThenInclude(p => p.BuyedWithProducts).ThenInclude(b => b.BuyedWith).ThenInclude(p => p.Category)
                    .Include(c => c.Product).ThenInclude(p => p.BuyedWithProducts).ThenInclude(b => b.BuyedWith).ThenInclude(p => p.Brand)
                    .Include(c => c.Product).ThenInclude(p => p.BuyedWithProducts).ThenInclude(b => b.BuyedWith).ThenInclude(p => p.Images).ThenInclude(i => i.Image);

            CartItem? cartItem = null;
            var viewModel = new CartItemViewModel();

            if (User.IsAuthenticated())
            {
                var user = await _userManager.GetUserAsync(User);

                cartItem = await cartItemQuery
                    .FirstOrDefaultAsync(c => c.ShoppingSession.User == user && c.Id == id, cancellationToken);
            } else
            {
                var shoppingSessionId = HttpContext.Session.GetShoppingSessionId();

                cartItem = await cartItemQuery
                    .FirstOrDefaultAsync(c => c.ShoppingSession.Id == shoppingSessionId && c.ShoppingSession.User == null, cancellationToken);
            }

            if (cartItem == null) return NotFound();

            viewModel = (CartItemViewModel)cartItem;

            return Ok(viewModel);
        }

        [HttpPost]
        [Route("add-to-cart")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddToCartViewModel>> AddToCart(AddToCartViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                #region Getting shopping session
                var shoppingSessionQuery = _context.ShoppingSessions
                    .Include(s => s.CartItems).ThenInclude(c => c.Product).ThenInclude(p => p.Category)
                    .Include(s => s.CartItems).ThenInclude(c => c.Product).ThenInclude(p => p.Brand)
                    .Include(s => s.CartItems).ThenInclude(c => c.Product).ThenInclude(p => p.Images).ThenInclude(i => i.Image);

                ShoppingSession? shoppingSession = null;

                if (User.IsAuthenticated())
                {
                    var user = await _userManager.GetUserAsync(User);

                    shoppingSession = await shoppingSessionQuery.FirstOrDefaultAsync(s => s.User == user, cancellationToken);

                    if (shoppingSession == null)
                    {
                        shoppingSession = new ShoppingSession
                        {
                            User = user,
                            CreatedAt = DateTimeOffset.Now
                        };

                        await _context.ShoppingSessions.AddAsync(shoppingSession, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                } else
                {
                    var shoppingSessionId = HttpContext.Session.GetShoppingSessionId();

                    shoppingSession = await _context.ShoppingSessions
                        .Include(s => s.CartItems)
                        .FirstOrDefaultAsync(s => s.Id == shoppingSessionId && s.User == null, cancellationToken);

                    if (shoppingSession == null)
                    {
                        shoppingSession = new ShoppingSession
                        {
                            CreatedAt = DateTimeOffset.Now
                        };

                        await _context.ShoppingSessions.AddAsync(shoppingSession, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);

                        HttpContext.Session.SetShoppingSessionId(shoppingSession.Id);
                    }
                }
                #endregion

                #region Cancel any current payment intent
                if (!string.IsNullOrEmpty(shoppingSession.CurrentStripePaymentIntentId))
                {
                    var paymentIntent = await _stripePaymentIntentService.Get(shoppingSession.CurrentStripePaymentIntentId);

                    string[] validPaymentIntentStatus = new string[] { "requires_payment_method" };

                    if (validPaymentIntentStatus.Contains(paymentIntent.Status))
                    {
                        await _stripePaymentIntentService.Cancel(shoppingSession.CurrentStripePaymentIntentId);

                        shoppingSession.CurrentStripePaymentIntentId = null;
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    else return BadRequest("Un pago se está procesando, por favor, cancela o termina este pago para seguir agregando productos al carrito.");
                }
                #endregion

                #region Adding cart item
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Images).ThenInclude(i => i.Image)
                    .Include(p => p.BuyedWithProducts).ThenInclude(b => b.BuyedWith).ThenInclude(p => p.Category)
                    .Include(p => p.BuyedWithProducts).ThenInclude(b => b.BuyedWith).ThenInclude(p => p.Brand)
                    .Include(p => p.BuyedWithProducts).ThenInclude(b => b.BuyedWith).ThenInclude(p => p.Images).ThenInclude(i => i.Image)
                    .FirstAsync(p => p.Id == model.ProductId, cancellationToken);

                var cartItem = shoppingSession.CartItems.FirstOrDefault(c => c.Product == product);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        Product = product,
                        UnitPrice = product.SalePrice,
                        UnitTaxes = product.SalePrice * product.VATPercentage,
                        Quantity = model.Quantity,
                        CreatedAt = DateTimeOffset.Now
                    };

                    shoppingSession.CartItems.Add(cartItem);
                } else
                {
                    cartItem.UnitPrice = product.SalePrice;
                    cartItem.UnitTaxes = product.SalePrice * product.VATPercentage;
                    cartItem.Quantity = model.SetQuantity ? model.Quantity : (cartItem.Quantity + model.Quantity);
                    cartItem.ModifiedAt = DateTimeOffset.Now;
                }

                await _context.SaveChangesAsync(cancellationToken);

                decimal subtotal = shoppingSession.CartItems.Sum(c => c.UnitPrice * c.Quantity);
                decimal stripeTotalFeesAmount = 0;

                var shouldTransferStripeFeesToCustomer = await _settingsService.GetAsync(Data.Enums.SettingType.STRIPE_TRANSFER_FEE);

                if (shouldTransferStripeFeesToCustomer == "true")
                {
                    var fee = decimal.Parse(await _settingsService.GetAsync(Data.Enums.SettingType.STRIPE_FIXED_FEE) ?? "0");
                    var feeRate = decimal.Parse(await _settingsService.GetAsync(Data.Enums.SettingType.STRIPE_PERCENT_FEE) ?? "0");
                    var taxRate = decimal.Parse(await _settingsService.GetAsync(Data.Enums.SettingType.STRIPE_VAT_PERCENT_OVER_FEE) ?? "0");

                    stripeTotalFeesAmount = StripeCalculator.GetFeeAmount(subtotal, feeRate, fee, taxRate);
                }

                shoppingSession.Total = subtotal + stripeTotalFeesAmount;
                shoppingSession.Taxes = shoppingSession.CartItems.Sum(c => c.UnitTaxes * c.Quantity);
                shoppingSession.Fees = stripeTotalFeesAmount;

                await _context.SaveChangesAsync(cancellationToken);
                #endregion

                return CreatedAtAction(nameof(GetById), new { id = cartItem.Id }, (CartItemViewModel)cartItem);
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Validation error: {ErrorMessage}", error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }
        } 
    }
}
