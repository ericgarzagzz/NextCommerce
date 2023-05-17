namespace NextCommerce.Utils
{
    public static class StripeCalculator
    {
        public static decimal GetGoalWithFeeAmount(decimal goalAmount, decimal feeRate, decimal fee, decimal taxRate) => (goalAmount + fee + (taxRate / 100) * fee) / (1 - (feeRate / 100) - (taxRate / 100) * (feeRate / 100));
        public static decimal GetFeeAmount(decimal goalAmount, decimal feeRate, decimal fee, decimal taxRate) => GetGoalWithFeeAmount(goalAmount, feeRate, fee, taxRate) - goalAmount;
    }
}
