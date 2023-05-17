using NextCommerce.Data.Entities;
using Stripe;

namespace NextCommerce.Extensions
{
    public static class AddressExtensions
    {
        public static string ToPlainAddress(this Address address) => $"{address.Line1}, {address.Line2}</br>{address.City}, {address.State}</br>{address.PostalCode}</br>{address.Country}";
        public static AddressInfo ToAddressInfo(this Shipping shipping) => new AddressInfo(shipping.Name, shipping.Address.Line1, shipping.Address.Line2, shipping.Address.PostalCode, shipping.Address.City, shipping.Address.State, shipping.Address.Country, shipping.Address.ToPlainAddress(), shipping.Phone, null);
        public static AddressInfo ToAddressInfo(this ChargeBillingDetails billingDetails) => new AddressInfo(billingDetails.Name ?? "", billingDetails.Address.Line1 ?? "", billingDetails.Address.Line2 ?? "", billingDetails.Address.PostalCode ?? "", billingDetails.Address.City ?? "", billingDetails.Address.State ?? "", billingDetails.Address.Country ?? "", billingDetails.Address.ToPlainAddress(), billingDetails.Phone ?? "", null);
    }
}
