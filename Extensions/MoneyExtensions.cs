namespace NextCommerce.Extensions
{
    public static class MoneyExtensions
    {
        public static long ToCents(this decimal amount)
        {
            return (long)(amount * 100);
        }

        public static decimal ToMexicanPesos(this long cents)
        {
            return cents / 100m;
        }

    }
}
