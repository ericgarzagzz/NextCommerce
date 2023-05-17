namespace NextCommerce.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Category? Parent { get; set; }
        public Image? SliderImage { get; set; }
        public int SliderOrder { get; set; }
        public bool IsNew { get; set; } = false;
    }
}
