// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Text.Json.Serialization;

namespace Saharaviewpoint.Models.View.Auth;

public class UserView
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Uid { get; set; }
    public string Type { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
}