using System.Collections;

namespace NextCommerce.Models.API
{
    public class CartViewModel
    {
        public decimal Total { get; set; }
        public string TotalStr => Total.ToString("C");
        public decimal Taxes { get; set; }
        public string TaxesStr => Taxes.ToString("C");
        public decimal Fees { get; set; }
        public string FeesStr => Fees.ToString("C");
        public decimal TotalQty => Items.Sum(i => i.Quantity);
        public IEnumerable<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    }
}
