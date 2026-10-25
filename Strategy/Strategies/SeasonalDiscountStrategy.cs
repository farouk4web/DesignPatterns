namespace Strategy.Strategies;

public class SeasonalDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal totalAmount)
    {
        return totalAmount * 0.90m; // 10% off
    }
}
