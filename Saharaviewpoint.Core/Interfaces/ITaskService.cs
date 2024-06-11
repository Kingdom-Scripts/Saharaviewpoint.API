using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Input.Task;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface ITaskService
{
    Task<Result> CreateTask(TaskModel model);
    Task<Result> ListTasks(TaskSearchModel request);
    Task<Result> GetTask(int taskId);
    Task<Result> ListAttachments(int taskId);
    Task<Result> AddAttachmentToTask(int taskId, FileUploadModel model, IProgress<int> progress);
    Task<Result> RemoveAttachmentFromTask(int taskId, int documentId);
    Task<Result> ListLogs(int taskId, PagingOptionModel request);
    Task<Result> AddComment(int taskId, CommentModel model);
    Task<Result> RemoveComment(int taskId, int commentId);
    Task<Result> ListComments(int taskId, PagingOptionModel request);
}