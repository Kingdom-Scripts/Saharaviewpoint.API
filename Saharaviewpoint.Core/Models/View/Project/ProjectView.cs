﻿using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.View.Project;

public class ProjectView
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string TypeName { get; set; }
    public required string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsPriority { get; set; }
    public int Order { get; internal set; }
    public int? AssigneeId { get; set; }

    public ProjectUserModel Assignee { get; set; }
    public ProjectUserModel CreatedBy { get; set; }

    [JsonIgnore]
    public bool IsDeleted { get; internal set; }
}

public class ProjectUserModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}