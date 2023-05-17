using NextCommerce.Data.Enums;

namespace NextCommerce.Data.Entities
{
    public class ProductShowcaseCollection
    {
        public int Id { get; set; }
        public string? Caption { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public ProductShowcaseType Type { get; set; }
        public ICollection<ProductShowcaseCollectionItem> Items { get; } = new List<ProductShowcaseCollectionItem>();
    }
}
