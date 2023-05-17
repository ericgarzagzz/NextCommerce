namespace NextCommerce.Data.Entities
{
    public class State
    {
        public int Id { get; set; }
        public Country Country { get; set; }
        public string Name { get; set; }
        public string? ExternalId { get; set; }
        public ICollection<City> Cities { get; } = new List<City>();
    }
}
