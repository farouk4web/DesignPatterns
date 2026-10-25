namespace Strategy.Strategies;

public class LoyaltyDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal totalAmount)
    {
        return totalAmount * 0.80m; // 20% off
    }
}
