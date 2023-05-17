namespace NextCommerce.Models.API
{
    public class BuyedWithProductItemViewModel
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string PriceStr => Price.ToString("C");
        public decimal SalePrice { get; set; }
        public string SalePriceStr => SalePrice.ToString("C");
        public string? ProductImage { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
    }
}
