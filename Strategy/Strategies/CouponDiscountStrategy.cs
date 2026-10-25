namespace Strategy.Strategies;

public class CouponDiscountStrategy(decimal discountPercentage) : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal totalAmount)
    {
        return totalAmount * (1 - discountPercentage / 100);
    }
}
