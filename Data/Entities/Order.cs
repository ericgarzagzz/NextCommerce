using System.ComponentModel.DataAnnotations.Schema;

namespace NextCommerce.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        [Column(TypeName = "money")]
        public decimal Total { get; set; }
        [Column(TypeName = "money")]
        public decimal Fees { get; set; }
        public OrderDetails Details { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ICollection<OrderLineItem> LineItems { get; } = new List<OrderLineItem>();
    }
}
