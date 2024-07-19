// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.App;

public class TaskAttachment : BaseAppModel
{
    public int TaskId { get; set; }
    public int DocumentId { get; set; }

    public SvpTask? Task { get; set; }
    public Document? Document { get; set; }
}