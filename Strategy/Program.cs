using Strategy.Strategies;

namespace Strategy;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Strategy Pattern — Checkout Discounts");
        Console.WriteLine();

        decimal cartTotal = 200.00m;
        var checkout = new CheckoutContext(new NoDiscountStrategy());

        Console.WriteLine($"Cart total: ${cartTotal:F2}");
        Console.WriteLine($"No discount: ${checkout.CalculateFinalTotal(cartTotal):F2}");

        checkout.SetStrategy(new SeasonalDiscountStrategy());
        Console.WriteLine($"Seasonal (10% off): ${checkout.CalculateFinalTotal(cartTotal):F2}");

        checkout.SetStrategy(new LoyaltyDiscountStrategy());
        Console.WriteLine($"Loyalty (20% off): ${checkout.CalculateFinalTotal(cartTotal):F2}");

        checkout.SetStrategy(new CouponDiscountStrategy(15m));
        Console.WriteLine($"Coupon (15% off): ${checkout.CalculateFinalTotal(cartTotal):F2}");
    }
}
