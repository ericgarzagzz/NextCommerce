using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextCommerce.Data.Entities
{
    [Index(nameof(UserId), IsUnique = true)]
    public class ShoppingSession
    {
        public Guid Id { get; set; }
        public ApplicationUser? User { get; set; }
        public Guid? UserId { get; set; }
        [Column(TypeName = "money")]
        public decimal Total { get; set; }
        [Column(TypeName = "money")]
        public decimal Taxes { get; set; }
        [Column(TypeName = "money")]
        public decimal Fees { get; set; }
        public string? CurrentStripePaymentIntentId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ICollection<CartItem> CartItems { get; } = new List<CartItem>();
    }
}
