namespace NextCommerce.Extensions
{
    public static class StripePaymentMethodIdExtensions
    {
        public static string ToHumanReadableString(this string paymentMethodId)
        {
            switch (paymentMethodId)
            {
                case "card":
                    return "Tarjeta de débito o crédito";
                case "apple_pay":
                    return "Apple Pay";
                case "google_pay":
                    return "Google Pay";
                case "microsoft_pay":
                    return "Microsoft Pay";
                case "alipay":
                    return "Alipay";
                case "wechat_pay":
                    return "WeChat Pay";
                case "sepa_debit":
                    return "SEPA Direct Debit";
                case "sofort":
                    return "SOFORT";
                case "giropay":
                    return "Giropay";
                case "ideal":
                    return "iDEAL";
                case "bancontact":
                    return "Bancontact";
                case "eps":
                    return "EPS";
                case "multibanco":
                    return "Multibanco";
                case "p24":
                    return "Przelewy24";
                case "fpx":
                    return "FPX";
                case "grabpay":
                    return "GrabPay";
                case "oxxo":
                    return "OXXO";
                case "pagofacil":
                    return "PagoFacil";
                case "rapipago":
                    return "Rapipago";
                default:
                    return "Método de pago no reconocido";
            }
        }
    }
}
