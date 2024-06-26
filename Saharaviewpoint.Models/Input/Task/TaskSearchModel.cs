using Saharaviewpoint.Models.Input;

namespace Saharaviewpoint.Models.Input.Task;

public class TaskSearchModel : PagingOptionModel
{
    public int ProjectId { get; set; }
    public List<string> Statuses { get; set; } = new();
    public List<string> Types { get; set; } = new();
}