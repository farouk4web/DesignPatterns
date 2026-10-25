namespace FactoryMethod.Processors;

public interface IPaymentProcessor
{
    void ProcessPayment(decimal amount);
}