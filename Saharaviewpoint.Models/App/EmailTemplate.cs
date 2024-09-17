// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class EmailTemplate : BaseAppModel
{
    [StringLength(25)]
    public string Key { get; set; }
    // [StringLength(255)]
    // public string Token { get; set; }
}