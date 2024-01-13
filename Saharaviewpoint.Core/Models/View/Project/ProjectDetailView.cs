namespace Saharaviewpoint.Core.Models.View.Project;

public class ProjectDetailView : ProjectView
{
    public string SizeOfSite { get; set; }

    public decimal Budget { get; set; }

    public string Location { get; set; }

    public int TypeId { get; set; }

    public string TypeName { get; set; }

    public int? DesignId { get; set; }

    public string SiteCondition { get; set; }

    public DocumentView Design { get; set; }
}