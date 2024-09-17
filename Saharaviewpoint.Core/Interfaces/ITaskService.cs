// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Task;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface ITaskService
{
    Task<Result> CreateTask(TaskModel model);
    Task<Result> ListTasks(TaskSearchModel request);
    Task<Result> ListBoardTasks(int projectId, string searchQuery);
    Task<Result> GetTask(int taskId);
    Task<Result> DeleteTask(int taskId);
    Task<Result> ListAttachments(int taskId);
    Task<Result> AddAttachmentToTask(int taskId, FileUploadModel model);
    Task<Result> GetVideoUploadToken();
    Task<Result> AddVideoToTask(int taskId, VideoDetailModel model);
    Task<Result> RemoveAttachmentFromTask(int taskId, int documentId);
    Task<Result> ListLogs(int taskId, PagingOptionModel request);
    Task<Result> ChangeTaskStatus(int taskId, TaskStatusModel model);
    Task<Result> ChangeDueDate(int taskId, TaskDueDateModel model);
    Task<Result> AddComment(int taskId, CommentModel model);
    Task<Result> RemoveComment(int taskId, int commentId);
    Task<Result> ListComments(int taskId, PagingOptionModel request);
}