namespace NextCommerce.Extensions
{
    public static class SessionExtensions
    {
        private const string _ShoppingSessionIdKey = "SHOPPING_SESSION_ID";

        public static Guid? GetShoppingSessionId(this ISession session)
        {
            string? shoppingSessionId = session.GetString(_ShoppingSessionIdKey);

            if (string.IsNullOrWhiteSpace(shoppingSessionId)) return null;
            else if (!Guid.TryParse(shoppingSessionId, out _)) return null;

            return Guid.Parse(shoppingSessionId);
        }

        public static void SetShoppingSessionId(this ISession session, Guid id)
        {
            session.SetString(_ShoppingSessionIdKey, id.ToString());
        }
    }
}
