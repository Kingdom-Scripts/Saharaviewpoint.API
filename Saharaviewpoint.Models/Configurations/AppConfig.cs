namespace Saharaviewpoint.Models.Configurations;

public class AppConfig
{
    public string TinifyKey { get; set; } = null!;
    public FileSettings FileSettings { get; set; } = null!;
    public BaseURLs BaseURLs { get; set; } = null!;
}

public class BaseURLs
{
    public string Api { get; set; } = null!;
    public string Admin { get; set; } = null!;
    public string Client { get; set; } = null!;
}