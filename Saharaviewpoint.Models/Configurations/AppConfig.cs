// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.Configurations;

public class AppConfig
{
    public string TinifyKey { get; set; }
    public ApiVideoConfig ApiVideo { get; set; }
    public FileSettings FileSettings { get; set; }
    public BaseURLs BaseURLs { get; set; }
}

public class BaseURLs
{
    public string Api { get; set; }
    public string Admin { get; set; }
    public string Client { get; set; }
    public string AssetBase { get; set; }
}