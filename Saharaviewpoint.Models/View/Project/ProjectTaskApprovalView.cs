// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.View.Project;

public class ProjectTaskApprovalView
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public required string ProjectTitle { get; set; }
    public DateTime RequestedOn { get; set; }
    public int RequesterId { get; set; }
    public required string RequesterName { get; set; }
    public bool IsFulfilled { get; set; }
    public int? FulfilledById { get; set; }
    public string FulfilledByName { get; set; }
    public bool? Status { get; set; }
    public DateTime? FulfilledOn { get; set; }
    public string Remark { get; set; }
}