namespace FactoryMethod.Processors;


public class FawryProcessor : IPaymentProcessor
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing ${amount} payment through Fawry.");
    }
}