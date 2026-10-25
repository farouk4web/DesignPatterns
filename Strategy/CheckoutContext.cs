namespace Strategy;

public class CheckoutContext
{
    private IDiscountStrategy _discountStrategy;

    public CheckoutContext(IDiscountStrategy discountStrategy)
    {
        ArgumentNullException.ThrowIfNull(discountStrategy);
        _discountStrategy = discountStrategy;
    }

    public void SetStrategy(IDiscountStrategy discountStrategy)
    {
        ArgumentNullException.ThrowIfNull(discountStrategy);
        _discountStrategy = discountStrategy;
    }

    public decimal CalculateFinalTotal(decimal cartTotal)
    {
        return _discountStrategy.ApplyDiscount(cartTotal);
    }
}
