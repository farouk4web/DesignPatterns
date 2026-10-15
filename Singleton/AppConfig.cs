namespace Singleton;

// Singleton pattern ensures a class has only one instance and provides a global point of access to it.

class AppConfig
{
    public string? AppName { get; }
    public string? Version { get; }
    public string? Environment { get; }
    public int UseCounter { get; set; }

    private AppConfig()
    {
        AppName = "Singleton Demo";
        Version = "1.0.0";
        Environment = "Development";
    }

    // initializ with double-check locking
    private static readonly Lock _lock = new();
    private static AppConfig? _instance;

    public static AppConfig Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (_lock)
                    _instance ??= new AppConfig();
            }
            return _instance;
        }
    }

    // initializ with Eager
    //private static readonly AppConfig _instance = new();
    //public static AppConfig Instance => _instance;

    // initializ with Lazy => The Best
    //private static readonly Lazy<AppConfig> _lazy = new(() => new AppConfig());
    //public static AppConfig Instance => _lazy.Value;
}