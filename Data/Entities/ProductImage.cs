namespace NextCommerce.Data.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public Image Image { get; set; }
        public int Order { get; set; }
    }
}
