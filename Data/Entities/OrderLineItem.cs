using System.ComponentModel.DataAnnotations.Schema;

namespace NextCommerce.Data.Entities
{
    public class OrderLineItem
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitTaxes { get; set; }
        public int Quantity { get; set; }
    }
}
