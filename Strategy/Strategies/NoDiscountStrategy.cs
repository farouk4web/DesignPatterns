namespace Strategy.Strategies;

public class NoDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal totalAmount)
    {
        return totalAmount;
    }
}