// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Text.Json.Serialization;

namespace Saharaviewpoint.Models.ApiVideo.Response;
public class ApiVideoTokenView
{
    public string Token { get; set; }
    [JsonIgnore]
    public int Ttl { get; set; }
}
