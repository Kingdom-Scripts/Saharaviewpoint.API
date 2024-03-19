namespace Saharaviewpoint.Core.Models.Configurations;

public class AppConfig
{
    public string TinifyKey { get; set; }
    public FileSettings FileSettings { get; set; }
    public BaseURLs BaseURLs { get; set; }
}

public class BaseURLs
{
    public string AdminClient { get; set; }
    public string MailerSend { get; set; }
}