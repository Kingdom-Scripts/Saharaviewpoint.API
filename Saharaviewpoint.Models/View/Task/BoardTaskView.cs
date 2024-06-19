namespace Saharaviewpoint.Models.View.Task;

public class BoardTaskView
{
    public int Id { get; set; }
    public string? Epic { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
    public required string Summary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }
    public int Order { get; set; }
}
