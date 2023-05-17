namespace NextCommerce.Data.Entities
{
    public class ProductSpecification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Product Product { get; set; }
    }
}
