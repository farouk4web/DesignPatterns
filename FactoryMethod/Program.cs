namespace FactoryMethod;

internal class Program
{
    static void Main(string[] args)
    {
        var paypal = PaymentProcessorFactory.CreateProcessor("paypal");
        paypal.ProcessPayment(100.00m);

        var stripe = PaymentProcessorFactory.CreateProcessor("stripe");
        stripe.ProcessPayment(250.50m);

        var fawry = PaymentProcessorFactory.CreateProcessor("fawry");
        fawry.ProcessPayment(75.25m);
    }
}
