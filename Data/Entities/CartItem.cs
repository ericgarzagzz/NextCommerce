using System.ComponentModel.DataAnnotations.Schema;

namespace NextCommerce.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public ShoppingSession ShoppingSession { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitTaxes { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
