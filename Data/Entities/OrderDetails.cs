using NextCommerce.Data.Enums;

namespace NextCommerce.Data.Entities
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string StripePaymentIntentStatus { get; set; }
        public string StripePaymentMethod { get; set; }
        public ApplicationUser? User { get; set; }
        public string Email { get; set; }
        public AddressInfo ShippingAddress { get; set; }
        public AddressInfo? BillingAddress { get; set; }
        public OrderStatus Status { get; set; }
    }
}
