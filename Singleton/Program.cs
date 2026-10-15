namespace Singleton
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Singleton Pattern Demo");

            // This code demonstrates the Singleton pattern by accessing a single instance of AppConfig.

            AppConfig? config1 = AppConfig.Instance;
            AppConfig? config2 = AppConfig.Instance;

            config1?.UseCounter = ++config1.UseCounter;
            config2?.UseCounter = ++config2.UseCounter;

            Console.WriteLine($"UseCounter of config 1: {config1?.UseCounter}");
            Console.WriteLine($"UseCounter of config 2: {config2?.UseCounter}");

            Console.WriteLine($"Is ReferenceEquals : " + ReferenceEquals(config1, config2));
        }
    }
}
