// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.View.Task;

public class TaskView
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
    public required string Summary { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpectedStartDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Order { get; set; }
}