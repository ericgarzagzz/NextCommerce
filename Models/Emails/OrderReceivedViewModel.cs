using NextCommerce.Data.Entities;
using System.Security.Policy;

namespace NextCommerce.Models.Emails
{
    public class OrderReceivedViewModel
    {
        public string? HeaderImageUrl { get; set; }
        public string CustomerName { get; set; }
        public string OrderId { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public List<LineItem> Items { get; set; } = new List<LineItem>();
        public ItemsSummary Summary { get; set; }
        public bool HideMediaLinks { get; set; }
        public bool ShowFacebook { get; set; }
        public string? FacebookLogoUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public bool ShowTwitter { get; set; }
        public string? TwitterLogoUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public bool ShowInsta { get; set; }
        public string? InstaLogoUrl { get; set; }
        public string? InstaUrl { get; set; }

        public class LineItem
        {
            public string? ImageUrl { get; set; }
            public string Name { get; set; }
            public string Quantity { get; set; }
            public decimal Price { get; set; }
            public string PriceStr => Price.ToString("C");
        }
        
        public class ItemsSummary
        {
            public decimal Products { get; set; }
            public string ProductsStr => Products.ToString("C");
            public decimal Tax { get; set; }
            public string TaxStr => Tax.ToString("C");
            public decimal Subtotal => Products + Tax;
            public string SubtotalStr => Subtotal.ToString("C");
            public decimal Fees { get; set; }
            public string FeesStr => Fees.ToString("C");
            public decimal Total { get; set; }
            public string TotalStr => Total.ToString("C");
        }
    }
}
