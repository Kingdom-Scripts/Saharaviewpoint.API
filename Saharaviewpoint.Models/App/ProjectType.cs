// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class ProjectType : BaseAppModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public int CreatedById { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedById { get; set; }

    public DateTime? DateDeleted { get; set; }
}
