using Saharaviewpoint.Core.Models.View.Project;

namespace Saharaviewpoint.Core.Models.View.Task;

public class TaskDetailView : TaskView
{
    public int CreatedById { get; set; }
    public int? UpdatedById { get; set; }

    public required ProjectView Project { get; set; }
    public required ReferenceUserView CreatedBy { get; set; }
    public ReferenceUserView? UpdatedBy { get; set; }
    public IEnumerable<DocumentView> Attachments { get; set; } = new List<DocumentView>();
}