namespace NextCommerce.Data.Entities
{
    public class BuyedWithProduct
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Product BuyedWith { get; set; }
        public int BuyedWithId { get; set; }
    }
}
