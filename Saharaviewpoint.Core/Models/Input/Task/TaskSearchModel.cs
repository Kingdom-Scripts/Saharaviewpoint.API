namespace Saharaviewpoint.Core.Models.Input.Project;

public class TaskSearchModel : PagingOptionModel
{
    public int ProjectId { get; set; }
    public string? SearchQuery { get; set; }
    public string? Status { get; set; }
}