using Microsoft.EntityFrameworkCore;

namespace NextCommerce.Data.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Image Logo { get; set; }
        public bool ShouldPromote { get; set; }
    }
}
