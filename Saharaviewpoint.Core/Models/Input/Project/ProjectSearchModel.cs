namespace Saharaviewpoint.Core.Models.Input.Project;

public class ProjectSearchModel : PagingOptionModel
{
    public string SearchQuery { get; set; }
    public string Status { get; set; }
    public DateTime? StartDueDate { get; set; }
    public DateTime? EndDueDate { get; set; }
    public bool PriorityOnly { get; set; } = false;
}