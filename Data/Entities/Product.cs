using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextCommerce.Data.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        [Column(TypeName = "money")]
        public decimal SalePrice { get; set; }
        public string? DocumentationLink { get; set; }
        public string? Tags { get; set; }
        public string? ModelNumber { get; set; }
        public string? SKU { get; set; }
        public decimal VATPercentage { get; set; }
        public int Rating { get; set; }
        public bool OnSale { get; set; } = true;
        public Image? SliderImage { get; set; }
        public string? SliderPrimaryColor { get; set; }
        public string? SliderSecondaryColor { get; set; }
        public int SliderOrder { get; set; }
        public bool ShowInSlider { get; set; }
        public Brand? Brand { get; set; }
        public Category? Category { get; set; }
        public ProductDimension Dimension { get; set; }
        public ICollection<BuyedWithProduct> BuyedWithProducts { get; } = new List<BuyedWithProduct>();
        public ICollection<ProductSpecification> Specifications { get; } = new List<ProductSpecification>();
        public ICollection<ProductImage> Images { get; } = new List<ProductImage>();
    }
}
