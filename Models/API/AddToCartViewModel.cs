using NextCommerce.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace NextCommerce.Models.API
{
    public class AddToCartViewModel
    {
        [Required(ErrorMessage = "No se ha proporcionado un identificador del producto.")]
        [SaleProductExists]
        public int ProductId { get; set; }
        [Required]
        [Range(0, 99999999999999)]
        public int Quantity { get; set; }
        public bool SetQuantity { get; set; }
    }
}
