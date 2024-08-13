// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.View.Task;

namespace Saharaviewpoint.Models.View.Project;

public class PtojectLogView : TaskLogView
{
    public int TaskId { get; set; }
    public string Remark { get; set; }
}
