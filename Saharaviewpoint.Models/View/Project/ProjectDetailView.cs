// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Newtonsoft.Json;

namespace Saharaviewpoint.Models.View.Project;

public class ProjectDetailView : ProjectView
{
    public string Description { get; set; }

    public required string SizeOfSite { get; set; }

    public decimal Budget { get; set; }

    public required string Location { get; set; }

    public int TypeId { get; set; }

    public int? DesignId { get; set; }

    public string SiteCondition { get; set; }

    public DocumentView? Design { get; set; }

    [JsonIgnore]
    public int CreatedById { get; set; }
}