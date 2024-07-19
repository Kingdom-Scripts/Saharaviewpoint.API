// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

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