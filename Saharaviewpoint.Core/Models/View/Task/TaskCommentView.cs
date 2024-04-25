namespace Saharaviewpoint.Core.Models.View.Task
{
    public class TaskCommentView
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Message { get; set; } = null!;
        public  DateTime CreatedAt { get; set; }
    }
}
