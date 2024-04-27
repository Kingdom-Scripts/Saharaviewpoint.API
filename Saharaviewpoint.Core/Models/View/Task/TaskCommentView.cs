using Saharaviewpoint.Core.Services;

namespace Saharaviewpoint.Core.Models.View.Task
{
    public class TaskCommentView
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int ParentId { get; set; }
        public string Message { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUid { get; set; }
        public IEnumerable<TaskCommentView> Children { get; set; } = new List<TaskCommentView>();
    }
}
