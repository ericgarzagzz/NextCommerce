namespace NextCommerce.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ExternalId { get; set; }
        public ICollection<State> States { get; } = new List<State>();
    }
}
