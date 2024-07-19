// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Text.Json.Serialization;

namespace Saharaviewpoint.Models.View.Project;

public class ProjectView
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string TypeName { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public bool IsPriority { get; set; }
    public int Order { get; set; }
    public int? AssigneeId { get; set; }

    public ReferenceUserView? Assignee { get; set; }
    public ReferenceUserView? CreatedBy { get; set; }

    [JsonIgnore]
    public bool IsDeleted { get; set; }
}