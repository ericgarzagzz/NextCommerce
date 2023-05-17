using NextCommerce.Data;
using System.ComponentModel.DataAnnotations;

namespace NextCommerce.Attributes.Validation
{
    public class SaleProductExistsAttribute : ValidationAttribute
    {protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = validationContext.GetRequiredService<ApplicationDbContext>();

            if (value == null) return new ValidationResult("No se ha proporcionado un identificador del producto.");
            var productId = (int)value;

            var product = context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null) return new ValidationResult("El producto seleccionado no se ha encontrado.");
            else if (!product.OnSale) return new ValidationResult("El producto seleccionado no está disponible.");

            return ValidationResult.Success;
        }
    }
}
