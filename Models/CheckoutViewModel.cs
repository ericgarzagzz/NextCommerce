using NextCommerce.Data.Entities;

namespace NextCommerce.Models
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel(ShoppingSession shoppingSession, string paymentIntentClientSecret)
        {
            ShoppingSession = shoppingSession;
            PaymentIntentClientSecret = paymentIntentClientSecret;
        }

        public ShoppingSession ShoppingSession { get; set; }
        public string PaymentIntentClientSecret { get; set; }
    }
}
