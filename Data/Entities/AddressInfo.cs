using NextCommerce.Data.Common;

namespace NextCommerce.Data.Entities
{
    public class AddressInfo : ValueObject
    {
        public AddressInfo(string fullName, string address, string neighborhood, string postalCode, string city, string state, string country, string plainAddress, string phoneNumber, string? instructions)
        {
            FullName = fullName;
            Address = address;
            Neighborhood = neighborhood;
            PostalCode = postalCode;
            City = city;
            State = state;
            Country = country;
            PlainAddress = plainAddress;
            PhoneNumber = phoneNumber;
            Instructions = instructions;
        }

        public string FullName { get; private set; }
        public string Address { get; private set; }
        public string Neighborhood { get; private set; }
        public string PostalCode { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string PlainAddress { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? Instructions { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FullName;
            yield return Address;
            yield return Neighborhood;
            yield return PostalCode;
            yield return City;
            yield return State;
            yield return Country;
            yield return PlainAddress;
            yield return PhoneNumber;
            yield return Instructions ?? string.Empty;
        }
    }
}
