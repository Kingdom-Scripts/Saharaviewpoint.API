// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.Input;

namespace Saharaviewpoint.Models.Input.Task;

public class TaskSearchModel : PagingOptionModel
{
    public int ProjectId { get; set; }
    public List<string> Statuses { get; set; } = new();
    public List<string> Types { get; set; } = new();
}