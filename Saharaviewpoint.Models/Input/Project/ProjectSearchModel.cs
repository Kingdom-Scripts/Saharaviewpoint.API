// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.Input.Project;

public class ProjectSearchModel : PagingOptionModel
{
    public string Status { get; set; }
    public DateTime? StartDueDate { get; set; }
    public DateTime? EndDueDate { get; set; }
    public bool PriorityOnly { get; set; } = false;
}