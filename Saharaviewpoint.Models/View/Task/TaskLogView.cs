namespace Saharaviewpoint.Models.View.Task
{
    public class TaskLogView
    {
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? PreviousState { get; set; }
        public string? CurrentState { get; set; }
    }
}