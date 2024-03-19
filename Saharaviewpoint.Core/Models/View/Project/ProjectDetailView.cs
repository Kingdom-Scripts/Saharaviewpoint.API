namespace Saharaviewpoint.Core.Models.View.Project;

public class ProjectDetailView : ProjectView
{
    public string? Description { get; set; }

    public required string SizeOfSite { get; set; }

    public decimal Budget { get; set; }

    public required string Location { get; set; }

    public int TypeId { get; set; }

    public int? DesignId { get; set; }

    public string? SiteCondition { get; set; }

    public DocumentView? Design { get; set; }
}