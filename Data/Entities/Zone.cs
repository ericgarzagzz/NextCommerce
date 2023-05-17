namespace NextCommerce.Data.Entities
{
    public class Zone
    {
        public int Id { get; set; }
        public City City { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public string? PostalCode { get; set; }
        public string? ExternalId { get; set; }
    }
}
