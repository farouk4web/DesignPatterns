namespace FactoryMethod.Processors;

public class PayPalProcessor : IPaymentProcessor
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing ${amount} payment through PayPal.");
    }
}
