using Microsoft.EntityFrameworkCore;

namespace NextCommerce.Data.Entities
{
    [Index(nameof(Path), IsUnique = true)]
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int SizeInBytes { get; set; }
        public bool IsRemote { get; set; } = false;
    }
}
