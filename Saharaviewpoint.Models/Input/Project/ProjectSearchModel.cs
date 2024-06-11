using Saharaviewpoint.Models.Input;

namespace Saharaviewpoint.Models.Input.Project;

public class ProjectSearchModel : PagingOptionModel
{
    public string Status { get; set; }
    public DateTime? StartDueDate { get; set; }
    public DateTime? EndDueDate { get; set; }
    public bool PriorityOnly { get; set; } = false;
}