using FactoryMethod.Processors;

namespace FactoryMethod;

public static class PaymentProcessorFactory
{
    public static IPaymentProcessor CreateProcessor(string provider)
    {
        return provider.ToLower() switch
        {
            "paypal" => new PayPalProcessor(),
            "stripe" => new StripeProcessor(),
            "fawry" => new FawryProcessor(),

            _ => throw new ArgumentException("Invalid payment provider"),
        };
    }
}
