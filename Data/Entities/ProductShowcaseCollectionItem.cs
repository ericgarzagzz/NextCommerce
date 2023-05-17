namespace NextCommerce.Data.Entities
{
    public class ProductShowcaseCollectionItem
    {
        public int Id { get; set; }
        public ProductShowcaseCollection Collection { get; set; }
        public Product Product { get; set; }
    }
}
