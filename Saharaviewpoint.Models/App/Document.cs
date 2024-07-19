// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class Document : BaseAppModel
{
    [Required]
    public string Name { get; set; }

    [StringLength(25)]
    [Required]
    public string Type { get; set; }

    [MaxLength(255)]
    public required string Url { get; set; }

    [MaxLength(255)]
    public required string ThumbnailUrl { get; set; }

    [Required]
    public int CreatedById { get; set; }

    public User? CreatedBy { get; set; }
}