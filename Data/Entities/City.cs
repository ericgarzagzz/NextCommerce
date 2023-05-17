namespace NextCommerce.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public State State { get; set; }
        public int StateId { get; set; }
        public string Name { get; set; }
        public string? ExternalId { get; set; }
        public ICollection<Zone> Zones { get; } = new List<Zone>();
    }
}
