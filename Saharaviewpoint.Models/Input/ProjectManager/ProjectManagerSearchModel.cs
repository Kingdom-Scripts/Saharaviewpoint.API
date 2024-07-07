
namespace Saharaviewpoint.Models.Input.ProjectManager;

public class ProjectManagerSearchModel : PagingOptionModel
{
    public DateTime? DateJoinedStart { get; set; }
    public DateTime? DateJoinedEnd { get; set; }
    public bool IsActiveOnly { get; set; } = false;
    public bool IsInactiveOnly { get; set; } = false;
}