// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class Business : BaseAppModel
{
    [Required]
    [MaxLength(10)]
    public string Code { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    public string Address { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [MaxLength(50)]
    public string ContactFirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string ContactLastName { get; set; }

    [Required]
    [MaxLength(25)]
    public string ContactPhone { get; set; }
}