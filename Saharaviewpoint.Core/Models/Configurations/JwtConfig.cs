namespace Saharaviewpoint.Core.Models.Configurations;

public class JwtConfig
{
    public string Secret { get; set; }
    public int Expires { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int RefreshExpireDays { get; set; }
    public string AllowedDomains { get; set; }
}