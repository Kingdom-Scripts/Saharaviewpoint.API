using Saharaviewpoint.Models.Input;

namespace Saharaviewpoint.Models.Input.Task;

public class TaskSearchModel : PagingOptionModel
{
    public int ProjectId { get; set; }
    public string? Status { get; set; }
}