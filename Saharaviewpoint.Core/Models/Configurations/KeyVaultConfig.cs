namespace Saharaviewpoint.Core.Models.Configurations;

public class KeyVaultConfig
{
    public required string KeyVaultURL { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string DirectoryID { get; set; }
}
