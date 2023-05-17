using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Entities;

namespace NextCommerce.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<WishlistController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("add-to-wishlist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Add(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product == null) return BadRequest("El producto no existe o no está disponible.");

            var isAlreadyInWishlist = await _context.Wishlist
                .AnyAsync(i => i.User == user && i.Product == product, cancellationToken);

            if (!isAlreadyInWishlist)
            {
                await _context.Wishlist.AddAsync(new WishItem
                {
                    Product = product,
                    User = user,
                    CreatedAt = DateTimeOffset.Now
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }

            return Ok();
        }

        [HttpPost]
        [Route("remove-from-wishlist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            var wishItem = await _context.Wishlist.FirstOrDefaultAsync(i => i.User == user && i.Product == product, cancellationToken);

            if (wishItem == null) return BadRequest("Este producto no existe o ya ha sido eliminado de tu lista de deseos anteriormente.");

            _context.Wishlist.Remove(wishItem);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}
