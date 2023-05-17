namespace NextCommerce.Models
{
    public class ProductSearchItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string? Image { get; set; }
        public bool ShowOldPrice { get; set; }
    }
}
