// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.Configurations;

public class AppConfig
{
    public string KeyVaultUrl { get; set; }
    public string ApiVideoUrl { get; set; }
    public FileSettings FileSettings { get; set; }
    public BaseUrLs BaseUrLs { get; set; }
}

public class BaseUrLs
{
    public string Api { get; set; }
    public string Admin { get; set; }
    public string Client { get; set; }
    public string AssetBase { get; set; }
}