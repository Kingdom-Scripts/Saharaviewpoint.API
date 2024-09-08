// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class ProjectTaskApproval : BaseAppModel
{
    public int ProjectId { get; set; }
    public int RequesterId { get; set; }
    public bool IsFulfilled { get; set; } = false;
    public int? FulfilledById { get; set; }
    public bool? Status { get; set; }
    public DateTime? FulfilledOn { get; set; }
    [StringLength(2000)]
    public string Remark { get; set; }

    public Project Project { get; set; }
    public User Requester { get; set; }
    public User FulfilledBy { get; set; }
}