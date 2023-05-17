using NextCommerce.Data.Entities;

namespace NextCommerce.Models.API
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitPriceStr => UnitPrice.ToString("C");
        public decimal UnitTaxes { get; set; }
        public string UnitTaxesStr => UnitTaxes.ToString("C");
        public decimal Total => UnitPrice * Quantity;
        public string TotalStr => Total.ToString("C");
        public string? ProductImage { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public IEnumerable<BuyedWithProductItemViewModel> BuyedWith { get; set; }

        public static explicit operator CartItemViewModel(CartItem cartItem)
        {
            return new CartItemViewModel
            {
                Id = cartItem.Id,
                Quantity = cartItem.Quantity,
                ProductName = cartItem.Product.Name,
                UnitPrice = cartItem.UnitPrice,
                UnitTaxes = cartItem.UnitTaxes,
                ProductImage = cartItem.Product.Images.FirstOrDefault()?.Image.Path,
                CategoryName = cartItem.Product.Category?.Name,
                BrandName = cartItem.Product.Brand?.Name,
                BuyedWith = cartItem.Product.BuyedWithProducts.Select(p => p.BuyedWith).Select(p => new BuyedWithProductItemViewModel
                {
                    ProductName = p.Name,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    ProductImage = p.Images.FirstOrDefault()?.Image.Path,
                    CategoryName = p.Category?.Name,
                    BrandName = p.Brand?.Name
                })
            };
        }
    }
}
