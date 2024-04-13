using Saharaviewpoint.Core.Models.Input;
using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Input.Task;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface ITaskService
{
    Task<Result> CreateTask(TaskModel model);
    Task<Result> ListTasks(TaskSearchModel request);
    Task<Result> GetTask(int taskId);
    Task<Result> ListAttachments(int taskId);
    Task<Result> AddAttachmentToTask(int taskId, FileUploadModel model, IProgress<int> progress);
    Task<Result> RemoveAttachmentFromTask(int taskId, int documentId);
}