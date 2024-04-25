using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Models.Input;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Input.Task;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View;
using Saharaviewpoint.Core.Models.View.Task;
using System.Runtime.InteropServices;

namespace Saharaviewpoint.Core.Services;

public class TaskService : ITaskService
{
    private readonly SaharaviewpointContext _context;
    private readonly UserSession _userSession;
    private readonly IFileService _fileService;
    private readonly ILogger<TaskService> _logger;

    public TaskService(SaharaviewpointContext context, UserSession userSession, IFileService fileService, ILogger<TaskService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> CreateTask(TaskModel model)
    {
        var project = await _context.Projects
            .Where(p => p.Id == model.ProjectId)
            .Select(p => new Project
            {
                FolderNames = p.FolderNames
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return new ErrorResult("Project not found");

        var mappedTask = model.Adapt<SvpTask>();
        mappedTask.Status = TaskStatusEnum.TODO;
        mappedTask.CreatedById = _userSession.UserId;
        mappedTask.TaskAttachments = new(); // remove the default empty attachment

        // upload attachments if any
        string folder = project.FolderNames.First();
        string subFolder = project.FolderNames.Last();
        var attachments = new List<TaskAttachment>();
        foreach (var file in model.Attachments)
        {
            var uploaded = await _fileService.UploadFileInternal(folder, subFolder, file);
            if (uploaded.Success)
            {
                attachments.Add(new TaskAttachment()
                {
                    Task = mappedTask,
                    Document = uploaded.Content
                });
            }
            else
            {
                _logger.LogError(uploaded.Message);
            }
        }

        if (attachments.Any())
            await _context.AddRangeAsync(attachments);

        // add log
        AddTaskLog(mappedTask, $"Task created by {_userSession.Name}");

        // add task
        await _context.AddAsync(mappedTask);

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult(StatusCodes.Status201Created, mappedTask.Adapt<TaskDetailView>())
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> ListTasks(TaskSearchModel request)
    {
        bool projectExistAndHaveAccess = await _context.Projects
            .Where(p => p.Id == request.ProjectId)
            .AnyAsync(p => _userSession.IsAnyAdmin || p.AssigneeId == _userSession.UserId || p.CreatedById == _userSession.UserId);

        if (!projectExistAndHaveAccess)
            return new ErrorResult("Project not found or you do not have access to view tasks in this project");

        var query = _context.Tasks
            .Where(t => t.ProjectId == request.ProjectId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(t => t.Status == request.Status);

        if (!string.IsNullOrEmpty(request.SearchQuery))
            query = query.Where(t => t.Summary.Contains(request.SearchQuery));

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .ProjectToType<TaskView>()
            .ToPaginatedListAsync(request.PageIndex, request.PageSize);

        return new SuccessResult(tasks);
    }

    public async Task<Result> GetTask(int taskId)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId)
            .ProjectToType<TaskDetailView>()
            .FirstOrDefaultAsync();

        if (task == null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Task not found");

        return new SuccessResult(task);
    }

    public async Task<Result> ListAttachments(int taskId)
    {
        var attachments = await _context.TaskAttachments
            .Where(ta => ta.TaskId == taskId)
            .Select(ta => ta.Document)
            .ProjectToType<DocumentView>()
            .ToListAsync();

        return new SuccessResult(attachments);
    }

    public async Task<Result> AddAttachmentToTask(int taskId, FileUploadModel model, IProgress<int> progress)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => new SvpTask
            {
                Id = t.Id,
                ProjectId = t.ProjectId
            }).FirstOrDefaultAsync();

        if (task is null)
            return new ErrorResult("Invalid task provided");

        var projectFolders = await _context.Projects
            .Where(p => p.Id == task.ProjectId)
            .Select(p => p.FolderNames)
            .FirstOrDefaultAsync();

        // var uploaded = await _fileService.UploadFileInternal(projectFolders!.First(), projectFolders!.Last(), model.File);
        var uploaded =
            await _fileService.UploadTaskAttachment(projectFolders!.First(), projectFolders!.Last(), model.File, progress);

        if (!uploaded.Success)
            return new ErrorResult(uploaded.Message);

        var attachment = new TaskAttachment
        {
            TaskId = taskId,
            Document = uploaded.Content
        };

        // Add log
        AddTaskLog(task, $"{_userSession.Name} added an attachment", "None", uploaded.Content.Name);

        await _context.AddAsync(attachment);

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult(StatusCodes.Status201Created, attachment.Document.Adapt<DocumentView>())
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> RemoveAttachmentFromTask(int taskId, int documentId)
    {
        var attachment = await _context.TaskAttachments
            .Include(ta => ta.Document)
            .Where(ta => ta.TaskId == taskId && ta.DocumentId == documentId)
            .FirstOrDefaultAsync();

        if (attachment is null)
            return new ErrorResult("Attachment not found");

        // get the project folders
        var projectFolders = await _context.TaskAttachments
            .Where(ta => ta.TaskId == taskId && ta.DocumentId == documentId)
            .Select(ta => ta.Task!.Project!.FolderNames)
            .FirstOrDefaultAsync();

        // get the file name from the document url
        string fileName = attachment.Document!.Url.Split('/').Last();

        // delete the file from azure
        var deleted = await _fileService.DeleteFile(projectFolders!.First(), projectFolders!.Last(), fileName);

        if (!deleted.Success)
            return new ErrorResult(deleted.Message);

        _context.Remove(attachment);
        _context.Remove(attachment.Document);

        // add log
        AddTaskLog(attachment.Task!, $"{_userSession.Name} removed an attachment", attachment.Document.Name);

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult()
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> ListLogs(int taskId, PagingOptionModel request) { 
        var logs = await _context.TaskLogs
            .Where(tl => tl.TaskId == taskId)
            .OrderByDescending(tl => tl.CreatedAt)
            .ProjectToType<TaskLogView>()
            .ToPaginatedListAsync(request.PageIndex, request.PageSize);

        return new SuccessResult(logs);
    }

    #region Comments

    public async Task<Result> AddComment(int taskId, CommentModel model)
    {
        // validate task
        var taskExist = await _context.Tasks
            .AnyAsync(t => t.Id == taskId && !t.IsDeleted);

        if (!taskExist)
            return new ErrorResult(StatusCodes.Status404NotFound, "Task not found");

        var comment = model.Adapt<TaskComment>();
        comment.TaskId = taskId;
        comment.CreatedById = _userSession.UserId;
        comment.FullName = _userSession.Name;

        await _context.AddAsync(comment);

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult(StatusCodes.Status201Created, comment.Adapt<CommentView>())
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> RemoveComment(int taskId, int commentId)
    {
        var comment = await _context.TaskComments
            .FirstOrDefaultAsync(tc => tc.Id == commentId && tc.TaskId == taskId);

        if (comment is null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Comment does not exist.");

        _context.Remove(comment);

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult()
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> ListComments(int taskId, PagingOptionModel request)
    {
        var query = _context.TaskComments
            .Where(tc => tc.TaskId == taskId && tc.ParentId == null)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchQuery))
            query = query.Where(tc => tc.Message.Contains(request.SearchQuery));

        var comments = await query
            .Include(tc => tc.CreatedBy)
            .Include(tc => tc.Children.Take(2))
            .OrderByDescending(tc => tc.CreatedAt)
            .ProjectToType<CommentView>()
            .ToPaginatedListAsync(request.PageIndex, request.PageSize);

        return new SuccessResult(comments);
    }

    #endregion

    #region Private Methods

    private async void AddTaskLog(SvpTask task, string description, string? previousState = null, string? currentState = null)
    {
        var log = new TaskLog
        {
            TaskId = task.Id,
            Description = description,
            PreviousState = previousState,
            CurrentState = currentState,
            CreatedById = _userSession.UserId
        };

        if (task.Id == 0) log.Task = task;

        await _context.AddAsync(log);
    }

    #endregion
}