// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.Configurations;

public class KeyVaultConfig
{
    public required string KeyVaultURL { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string DirectoryID { get; set; }
}
