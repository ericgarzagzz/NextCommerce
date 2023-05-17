namespace NextCommerce.Data.Entities
{
    public class WishItem
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
