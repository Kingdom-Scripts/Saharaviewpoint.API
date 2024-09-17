// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.View.Task;

public class TaskDetailView : TaskView
{
    public int CreatedById { get; set; }
    public int? UpdatedById { get; set; }

    // public required ProjectView Project { get; set; }
    public required ReferenceUserView CreatedBy { get; set; }
    // public ReferenceUserView? UpdatedBy { get; set; }
}