// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Text;
using LazyCache;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Saharaviewpoint.Core.Contants;
using Saharaviewpoint.Core.Contants.CacheKeys;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.ApiVideo.Response;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Input.Task;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View;
using Saharaviewpoint.Models.View.Task;
using Serilog;

namespace Saharaviewpoint.Core.Services;

public class TaskService : BaseService, ITaskService
{
    private readonly SaharaviewpointContext _context;
    private readonly UserSession _userSession;
    private readonly IFileService _fileService;
    private readonly IAppCache _cache;
    private readonly IEmailService _emailService;
    private readonly BaseUrLs _baseUrls;
    private readonly HttpClient _apiVideoClient;
    private readonly ILogger _logger;

    public TaskService(SaharaviewpointContext context, UserSession userSession, IFileService fileService, IAppCache cache, IEmailService emailService, IOptions<AppConfig> options, IHttpClientFactory factory, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _baseUrls = options.Value.BaseUrLs;
        _apiVideoClient = factory.CreateClient(HttpClientKeys.ApiVideo);
    }

    public async Task<Result> CreateTask(TaskModel model)
    {
        var project = await _context.Projects
            .Where(p => p.Id == model.ProjectId)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.FolderNames
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return new ErrorResult("Project not found");

        var mappedTask = model.Adapt<SvpTask>();
        mappedTask.Status = TaskStatusEnum.TODO;
        mappedTask.ParentId = model.ParentId != 0 ? model.ParentId : null;
        mappedTask.CreatedById = _userSession.UserId;
        mappedTask.TaskAttachments = []; // remove the default empty attachment

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
                Log.Error("Failed to upload attachment for task: {@Task}. Error: {@Error}", mappedTask, uploaded.Message);
            }
        }

        if (attachments.Count != 0)
            await _context.AddRangeAsync(attachments);

        // add log
        AddTaskLog(mappedTask, $"Task created by {_userSession.Name}");

        // add task
        await _context.AddAsync(mappedTask);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.TaskListCacheKeys(),
            CacheKeys.BoardTasks(model.ProjectId),
            CacheKeys.BoardTasksValidation(model.ProjectId));

        // Send Notification Email
        {
            var emailRequest = new GenericEmailModel
            {
                To = _emailService.GetUserEmails(RolesConstants.SvpAdmin, RolesConstants.SuperAdmin),
                Subject = $"{project.Title} - New Task Created",
                Salutation = "Hello,",
                PrimaryMessage = $"This is to notify you that <strong>{_userSession.Name}</strong> created a new task in the project <strong>{project.Title}</strong>.<br><br>" +
                "<strong><span style=\"font-size:larger;\">Task Details</span></strong><br>" +
                $"<strong>Summary:</strong> {mappedTask.Summary}<br>" +
                $"<strong>Description:</strong> {mappedTask.Description}<br>" +
                $"<strong>Expected Start Date:</strong> {mappedTask.ExpectedStartDate:dd MMM, yyyy}<br>" +
                $"<strong>Expected End Date:</strong> {mappedTask.DueDate:dd MMMM, yyyy}<br>",
                SecondaryMessage = "You are getting this email as an admin because this project tasks has already been approved before now.",
                ClosingRemark = "Regards",
                ActionButton = new()
                {
                    Text = "View Task Details",
                    Url = $"{_baseUrls.Admin}/tasks/all?projectId={project.Id}&taskId={mappedTask.Id}"
                }
            };

            await _emailService.SendEmail(emailRequest);
        }

        return new SuccessResult(StatusCodes.Status201Created, mappedTask.Adapt<TaskDetailView>());
    }

    public async Task<Result> ListTasks(TaskSearchModel request)
    {
        string cacheKey = GenerateCacheKey(request, _userSession.UserId, _userSession.IsAnySvpAdmin);
        string validationCacheKey = $"{cacheKey}-validation";

        // Retrieve the current list of cache keys and add the new key
        var cacheKeys = _cache.GetOrAdd(CacheKeys.TaskListCacheKeys(), () => new List<string>(), new TimeSpan(0, 45, 0));
        if (!cacheKeys.Contains(cacheKey))
        {
            cacheKeys.Add(cacheKey);
            _cache.Add(CacheKeys.TaskListCacheKeys(), cacheKeys);
        }
        if (!cacheKeys.Contains(validationCacheKey))
        {
            cacheKeys.Add(validationCacheKey);
            _cache.Add(CacheKeys.TaskListCacheKeys(), cacheKeys);
        }

        bool projectExistAndHaveAccess = await _cache.GetOrAddAsync(validationCacheKey, async () =>
        {
            return await _context.Projects
            .Where(p => p.Id == request.ProjectId)
            .AnyAsync(p => _userSession.IsAnySvpAdmin || p.AssigneeId == _userSession.UserId || p.CreatedById == _userSession.UserId);
        }, new TimeSpan(0, 45, 0));

        if (!projectExistAndHaveAccess)
            return new ErrorResult("Project not found or you do not have access to view tasks in this project");

        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            var query = _context.Tasks
                .Where(t => t.ProjectId == request.ProjectId)
                .Where(t => !t.IsDeleted)
                .AsQueryable();

            // filter by status
            query = query.Where(t => request.Statuses.Count == 0
                        || request.Statuses.Contains(t.Status));

            // filter by type
            query = query.Where(t => request.Types.Count == 0
                       || request.Types.Contains(t.Type));

            if (!string.IsNullOrEmpty(request.SearchQuery))
                query = query.Where(t => t.Summary.Contains(request.SearchQuery));

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ProjectToType<TaskView>()
                .ToPaginatedListAsync(request.PageIndex, request.PageSize);
        }, new TimeSpan(0, 45, 0));

        return new SuccessResult(cachedResult);
    }

    public async Task<Result> GetTask(int taskId)
    {
        var cachedData = await _cache.GetOrAddAsync(CacheKeys.TaskDetails(taskId), async () =>
        {
            return await _context.Tasks
            .Where(t => t.Id == taskId && !t.IsDeleted)
            .ProjectToType<TaskDetailView>()
            .FirstOrDefaultAsync();
        }, new TimeSpan(0, 45, 0));

        if (cachedData == null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Task not found");

        return new SuccessResult(cachedData);
    }

    public async Task<Result> DeleteTask(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task is not null)
        {
            _context.Remove(task);

            // clear caches
            _cache.ClearCaches(CacheKeys.TaskListCacheKeys(),
                CacheKeys.TaskDetails(taskId),
                CacheKeys.BoardTasks(task.ProjectId),
                CacheKeys.BoardTasksValidation(task.ProjectId));
        }
        await _context.SaveChangesAsync();

        return new SuccessResult();
    }

    public async Task<Result> ListAttachments(int taskId)
    {
        var cachedData = await _cache.GetOrAddAsync(CacheKeys.ListAttachments(taskId), async () =>
        {
            return await _context.TaskAttachments
                .Where(ta => ta.TaskId == taskId)
                .Select(ta => new DocumentView
                {
                    Id = ta.Document.Id,
                    Name = ta.Document.Name,
                    Type = ta.Document.Type,
                    Url = ta.Document.Type == DocumentTypes.VIDEO ? ta.Document.Url : $"{_baseUrls.AssetBase}/{ta.Document.Url}",
                    ThumbnailUrl = ta.Document.Type == DocumentTypes.VIDEO ? ta.Document.ThumbnailUrl : $"{_baseUrls.AssetBase}/{ta.Document.ThumbnailUrl}",
                    CreatedAt = ta.Document.CreatedAt
                })
                .ToListAsync();
        }, new TimeSpan(0, 45, 0));

        return new SuccessResult(cachedData);
    }

    public async Task<Result> AddAttachmentToTask(int taskId, FileUploadModel model)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId)
            .Select(t => new
            {
                t.Id,
                t.ProjectId,
                t.Summary,
                TaskOwnerId = t.CreatedById,
                TaskOwnerEmail = t.CreatedBy.Email,
                TaskOwnerName = $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}",
                ProjectOwnerEmail = t.Project.CreatedBy.Email,
                ProjectOwnerName = $"{t.Project.CreatedBy.FirstName} {t.Project.CreatedBy.LastName}",

            }).FirstOrDefaultAsync();

        if (task is null)
            return new ErrorResult("Invalid task provided");

        var projectFolders = await _context.Projects
            .Where(p => p.Id == task.ProjectId)
            .Select(p => p.FolderNames)
            .FirstOrDefaultAsync();

        var uploaded = await _fileService.UploadFileInternal(projectFolders!.First(), projectFolders!.Last(), model.File);

        if (!uploaded.Success)
            return new ErrorResult(uploaded.Message);

        var attachment = new TaskAttachment
        {
            TaskId = taskId,
            Document = uploaded.Content
        };

        // Add log
        AddTaskLog(new SvpTask{Id = task.Id}, $"{_userSession.Name} added an attachment", "None", uploaded.Content.Name);

        await _context.AddAsync(attachment);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.ListAttachments(taskId));

        // Send Notification
        var emailModel = new GenericEmailModel
        {
            To = [new EmailAddress { Address = task.ProjectOwnerEmail, Name = task.ProjectOwnerName }],
            Cc = [new EmailAddress { Address = task.TaskOwnerEmail, Name = task.TaskOwnerName }],
            Subject = $"Attachment Uploaded - {task.Summary}",
            Salutation = "Hello,",
            PrimaryMessage =
                $"This is to notify you that <strong>{_userSession.Name}</strong> uploaded an attachment to the task <strong>{task.Summary}</strong>.",
            ClosingRemark = "Regards",
            ActionButton = new EmailActionButton
            {
                Text = "View Task",
                Url = $"{_baseUrls.Client}/project/task/{taskId}"
            },
            Attachments = [model.File]
        };
        if (task.TaskOwnerId != _userSession.UserId)
            emailModel.Cc.Add(new EmailAddress
            {
                Address = _context.Users.First(u => u.Id == _userSession.UserId).Email ,
                Name = _userSession.Name
            });

        await _emailService.SendEmail(emailModel);

        var result = attachment.Document.Adapt<DocumentView>();
        result.Url = result.Type == DocumentTypes.VIDEO ? result.Url : $"{_baseUrls.AssetBase}/{result.Url}";
        result.ThumbnailUrl = result.Type == DocumentTypes.VIDEO ? result.ThumbnailUrl : $"{_baseUrls.AssetBase}/{result.ThumbnailUrl}";

        return new SuccessResult(StatusCodes.Status201Created, result);
    }

    public async Task<Result> GetVideoUploadToken()
    {
        var model = new { ttl = 3600 };

        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

        var response = await _apiVideoClient.PostAsync("upload-tokens", content);
        if (!response.IsSuccessStatusCode)
            return new ErrorResult("Failed to get video upload token");

        string contentRes = await response.Content.ReadAsStringAsync();
        var uploadToken = JsonConvert.DeserializeObject<ApiVideoTokenView>(contentRes);

        return new SuccessResult(uploadToken);
    }

    public async Task<Result> AddVideoToTask(int taskId, VideoDetailModel model)
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

        var attachment = new TaskAttachment
        {
            TaskId = taskId,
            Document = new Document
            {
                Name = model.title,
                Type = DocumentTypes.VIDEO,
                Url = model.assets.mp4,
                ThumbnailUrl = model.assets.thumbnail,
                CreatedById = _userSession.UserId,
                VideoId = model.videoId,
                VideoDuration = model.Duration,
                VideoAsset = new()
                {
                    Iframe = model.assets.iframe,
                    Player = model.assets.player,
                    Hls = model.assets.hls,
                    Thumbnail = model.assets.thumbnail,
                    Mp4 = model.assets.mp4
                }
            }
        };

        // Add log
        AddTaskLog(task, $"{_userSession.Name} added a video", "None", attachment.Document.Name);

        await _context.AddAsync(attachment);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.ListAttachments(taskId));

        var result = attachment.Document.Adapt<DocumentView>();
        result.Url = result.Type == DocumentTypes.VIDEO ? result.Url : $"{_baseUrls.AssetBase}/{result.Url}";
        result.ThumbnailUrl = result.Type == DocumentTypes.VIDEO ? result.ThumbnailUrl : $"{_baseUrls.AssetBase}/{result.ThumbnailUrl}";

        return new SuccessResult(StatusCodes.Status201Created, result);
    }

    public async Task<Result> RemoveAttachmentFromTask(int taskId, int documentId)
    {
        var attachment = await _context.TaskAttachments
            .Include(ta => ta.Task)
            .Include(ta => ta.Document)
            .Where(ta => ta.TaskId == taskId && ta.DocumentId == documentId)
            .FirstOrDefaultAsync();

        if (attachment is null)
            return new ErrorResult("Attachment not found");

        if (attachment.Document.Type == DocumentTypes.VIDEO)
        {
            string videoId = attachment.Document.VideoId;
            var response = await _apiVideoClient.DeleteAsync($"videos/{videoId}");
            if (!response.IsSuccessStatusCode)
            {
                string contentString = await response.Content.ReadAsStringAsync();
                object error = JsonConvert.DeserializeObject<object>(contentString);
                _logger.Error("Failed to delete video with ID: {@VideoId}. {@Error}", videoId, error);
                return new ErrorResult("Failed to remove video, please try again later.");
            }
        }
        else
        {
            // get the project folders
            var projectFolders = await _context.TaskAttachments
                .Where(ta => ta.TaskId == taskId && ta.DocumentId == documentId)
                .Select(ta => ta.Task!.Project!.FolderNames)
                .FirstOrDefaultAsync();

            // get the file name from the document url
            string fileName = attachment.Document!.Url.Split('/').Last();

            // delete the file from azure
            var deletedRes = await _fileService.DeleteFile(projectFolders!.First(), projectFolders!.Last(), fileName);

            if (!deletedRes.Success && deletedRes.Message != "File not found")
                return new ErrorResult(deletedRes.Message);
        }

        _context.Remove(attachment);
        _context.Remove(attachment.Document);

        // add log
        AddTaskLog(attachment.Task!, $"{_userSession.Name} removed an attachment", attachment.Document.Name);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.ListAttachments(taskId));

        return new SuccessResult();
    }

    public async Task<Result> ListLogs(int taskId, PagingOptionModel request)
    {
        string generatedKey = GenerateCacheKey(request);
        string cacheKey = $"TaskService-ListLogs-{taskId}-{generatedKey}";

        // Retrieve the current list of cache keys and add the new key
        var cacheKeys = _cache.GetOrAdd(CacheKeys.TaskLogsCacheKeys(taskId), () => new List<string>(), new TimeSpan(0, 45, 0));
        if (!cacheKeys.Contains(cacheKey))
        {
            cacheKeys.Add(cacheKey);
            _cache.Add(CacheKeys.TaskLogsCacheKeys(taskId), cacheKeys);
        }
        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            return await _context.TaskLogs
                .Where(tl => tl.TaskId == taskId)
                .OrderByDescending(tl => tl.CreatedAt)
                .ProjectToType<TaskLogView>()
                .ToPaginatedListAsync(request.PageIndex, request.PageSize);
        }, new TimeSpan(0, 45, 0));

        return new SuccessResult(cachedResult);
    }

    public async Task<Result> ListBoardTasks(int projectId)
    {
        bool projectExistAndHaveAccess = await _cache.GetOrAddAsync(CacheKeys.BoardTasksValidation(projectId), async () =>
        {
            return await _context.Projects
                .Where(p => p.Id == projectId)
                .AnyAsync(p => _userSession.IsAnySvpAdmin
                    || p.AssigneeId == _userSession.UserId
                    || p.CreatedById == _userSession.UserId);
        }, new TimeSpan(0, 45, 0));

        if (!projectExistAndHaveAccess)
            return new ErrorResult("Project not found or you do not have access to view tasks in this project");

        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(CacheKeys.BoardTasks(projectId), async () =>
        {
            var query = from task in _context.Tasks
                        where !task.IsDeleted && task.ProjectId == projectId
                        where _userSession.IsAnySvpAdmin || task.Project!.AssigneeId == _userSession.UserId || task.CreatedById == _userSession.UserId
                        where task.Type != TaskTypeEnum.EPIC
                        select new
                        {
                            task,
                            task.Parent
                        };

            var results = await query
                .AsNoTracking()
                .OrderBy(t => t.task.Order)
                .ToListAsync();

            return results.Select(t => new BoardTaskView
            {
                Id = t.task.Id,
                Epic = t.Parent?.Type == "Epic" ? t.Parent.Summary : null,
                Type = t.task.Type,
                Status = t.task.Status,
                Summary = t.task.Summary,
                CreatedAt = t.task.CreatedAt,
                DueDate = t.task.DueDate,
                Order = t.task.Order
            }).ToList();
        }, new TimeSpan(0, 45, 0));

        return new SuccessResult(cachedResult);
    }

    public async Task<Result> ChangeTaskStatus(int taskId, TaskStatusModel model)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId)
            .Include(t => t.Parent)
            .FirstOrDefaultAsync();

        if (task is null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Task not found");

        if (task.Status == model.Status)
            return new ErrorResult("Task is already in the selected status");

        // make sure task is not jumping a status
        if (!TaskStatusEnum.IsValidTransition(task.Status, model.Status))
            return new ErrorResult("Invalid status transition");

        string previousState = task.Status;
        task.Status = model.Status;
        if (!task.StartDate.HasValue)
            task.StartDate = DateTime.UtcNow;

        // check if status is going back and check for reasons
        bool taskIsGoingBack = TaskStatusEnum.IsGoingBack(previousState, model.Status);
        if (taskIsGoingBack)
        {
            if (string.IsNullOrEmpty(model.Reason))
                return new ErrorResult("Reason is required when going back to previous status");
        }

        if (task.Status == TaskStatusEnum.COMPLETED)
            task.DateCompleted = DateTime.UtcNow;

        // move epic if task belongs to an epic
        if (task.Status == TaskStatusEnum.IN_PROGRESS && task.Parent != null && task.Parent.Type == TaskTypeEnum.EPIC && task.Parent.Status != TaskStatusEnum.IN_PROGRESS)
        {
            task.Parent.Status = TaskStatusEnum.IN_PROGRESS;
            AddTaskLog(task.Parent, $"{_userSession.Name} changed task status to {TaskStatusEnum.IN_PROGRESS}");
        }

        // add log
        AddTaskLog(task, $"{_userSession.Name} changed task status to {model.Status}", previousState, model.Status, model.Reason);

        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedById = _userSession.UserId;

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.TaskListCacheKeys(),
            CacheKeys.TaskDetails(taskId),
            CacheKeys.BoardTasks(task.ProjectId),
            CacheKeys.BoardTasksValidation(task.ProjectId));

        // Send Notification Email
        {
            var data = await _context.Projects
                .Where(p => p.Id == task.ProjectId)
                .Select(p => new
                {
                    ProjectTitle = p.Title,
                    OwnerEmail = p.CreatedBy!.Email,
                    OwnerFirstName = p.CreatedBy.FirstName,
                    OwnerLastName = p.CreatedBy.LastName
                }).FirstAsync();

            var emailRequest = new GenericEmailModel
            {
                To = [new EmailAddress{Address = data.OwnerEmail, Name = $"{data.OwnerFirstName} {data.OwnerLastName}"}],
                Subject = $"{data.ProjectTitle} - Task Update",
                Salutation = $"Hello {data.OwnerFirstName},",
                PrimaryMessage = $"This is to notify you that <strong>{_userSession.Name}</strong> changed the status of a task in the project <strong>{data.ProjectTitle}</strong>.<br><br>" +
                "<strong><span style=\"font-size:larger;\">Task Details</span></strong><br>" +
                    $"<strong>Task Summary:</strong> {task.Summary}<br>" +
                    $"<strong>Previous Status:</strong> {previousState}<br>" +
                    $"<strong>New Status:</strong> {task.Status}<br>" +
                    $"{(taskIsGoingBack ? $"<strong>Remark:</strong> {model.Reason}<br>" : "")}",
                ClosingRemark = "Regards",
                ActionButton = new()
                {
                    Text = "View Task",
                    Url = $"{_baseUrls.Client}/project/task/{taskId}"
                }
            };

            await _emailService.SendEmail(emailRequest);
        }

        return new SuccessResult(task.Adapt<TaskView>());
    }

    public async Task<Result> ChangeDueDate(int taskId, TaskDueDateModel model)
    {
        var task = await _context.Tasks
            .Where(t => t.Id == taskId)
            .FirstOrDefaultAsync();

        if (task is null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Task not found");

        if (task.Status == TaskStatusEnum.COMPLETED)
            return new ErrorResult("Task is already completed, cannot change due date.");

        if (task.DueDate == model.DueDate)
            return new ErrorResult("Task is already due on the selected date");

        var previousDue = task.DueDate;
        task.DueDate = model.DueDate;
        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedById = _userSession.UserId;

        // add log
        AddTaskLog(task, $"{_userSession.Name} changed due date to {model.DueDate:MMM dd, yyyy}", previousDue.ToString("MMM dd, yyyy"), task.DueDate.ToString("MMM dd, yyyy"), model.Reason);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.TaskListCacheKeys(),
            CacheKeys.TaskDetails(taskId),
            CacheKeys.BoardTasks(task.ProjectId),
            CacheKeys.BoardTasksValidation(task.ProjectId));

        // Send Notification Email
        {
            var data = await _context.Projects
                .Where(p => p.Id == task.ProjectId)
                .Select(p => new
                {
                    ProjectTitle = p.Title,
                    OwnerEmail = p.CreatedBy!.Email,
                    OwnerFirstName = p.CreatedBy.FirstName,
                    OwnerLastName = p.CreatedBy.LastName
                }).FirstAsync();

            var emailRequest = new GenericEmailModel
            {
                To = [new EmailAddress{Address = data.OwnerEmail, Name = $"{data.OwnerFirstName} {data.OwnerLastName}"}],
                Subject = $"{data.ProjectTitle} - Task Due Date Update",
                Salutation = $"Hello {data.OwnerFirstName},",
                PrimaryMessage = $"This is to notify you that <strong>{_userSession.Name}</strong> changed the due date of a task in the project <strong>{data.ProjectTitle}</strong>.<br><br>" +
                    $"<strong>Task Summary:</strong> {task.Summary}<br>" +
                    $"<strong>Former Due Date:</strong> {previousDue:dd MMM, yyyy}<br>" +
                    $"<strong>New Due Date:</strong> {task.DueDate:dd MMM, yyyy}<br>" +
                    $"<strong>Remark:</strong> {task.Status}<br>",
                ClosingRemark = "Regards",
                ActionButton = new()
                {
                    Text = "View Project",
                    Url = $"{_baseUrls.Client}/project/task/{taskId}"
                }
            };

            var adminEmail = emailRequest;
            adminEmail.To = _emailService.GetUserEmails(RolesConstants.SvpAdmin, RolesConstants.SuperAdmin);
            adminEmail.Salutation = "Hello,";
            adminEmail.ActionButton = new()
            {
                Text = "View Task",
                Url = $"{_baseUrls.Admin}/tasks/all?projectId={task.ProjectId}&taskId={taskId}"
            };

            await _emailService.SendEmail(emailRequest);
            await _emailService.SendEmail(adminEmail);
        }

        return new SuccessResult(task.Adapt<TaskView>());
    }

    #region Comments

    public async Task<Result> AddComment(int taskId, CommentModel model)
    {
        // validate task
        bool taskExist = await _context.Tasks
            .AnyAsync(t => t.Id == taskId && !t.IsDeleted);

        if (!taskExist)
            return new ErrorResult(StatusCodes.Status404NotFound, "Task not found");

        var comment = model.Adapt<TaskComment>();
        comment.TaskId = taskId;
        comment.CreatedById = _userSession.UserId;
        comment.FullName = _userSession.Name;

        await _context.AddAsync(comment);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.CommentListCacheKeys(taskId));

        return new SuccessResult(StatusCodes.Status201Created, comment.Adapt<TaskCommentView>());
    }

    public async Task<Result> RemoveComment(int taskId, int commentId)
    {
        var comment = await _context.TaskComments
            .FirstOrDefaultAsync(tc => tc.Id == commentId && tc.TaskId == taskId);

        if (comment is null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Comment does not exist.");

        _context.Remove(comment);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        // clear caches
        _cache.ClearCaches(CacheKeys.CommentListCacheKeys(taskId));

        return new SuccessResult();
    }

    public async Task<Result> ListComments(int taskId, PagingOptionModel request)
    {
        string generatedKey = GenerateCacheKey(request);
        string cacheKey = $"TaskService-Comment-{taskId}-{generatedKey}";

        // Retrieve the current list of cache keys and add the new key
        var cacheKeys = _cache.GetOrAdd(CacheKeys.CommentListCacheKeys(taskId), () => new List<string>(), new TimeSpan(0, 45, 0));
        if (!cacheKeys.Contains(cacheKey))
        {
            cacheKeys.Add(cacheKey);
            _cache.Add(CacheKeys.CommentListCacheKeys(taskId), cacheKeys);
        }
        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            var query = _context.TaskComments
            .Where(tc => tc.TaskId == taskId && tc.ParentId == null)
            .AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchQuery))
                query = query.Where(tc => tc.Message.Contains(request.SearchQuery));
            return await query
                 .Include(tc => tc.CreatedBy)
                 .Include(tc => tc.Children.Take(2))
                 .OrderByDescending(tc => tc.CreatedAt)
                 .ProjectToType<TaskCommentView>()
                 .ToPaginatedListAsync(request.PageIndex, request.PageSize);
        }, new TimeSpan(0, 45, 0));

        return new SuccessResult(cachedResult);
    }

    #endregion

    #region Private Methods

    private async void AddTaskLog(SvpTask task, string description, string previousState = null, string currentState = null, string remark = null)
    {
        var log = new TaskLog
        {
            TaskId = task.Id,
            Description = description,
            Remark = remark,
            PreviousState = previousState,
            CurrentState = currentState,
            CreatedById = _userSession.UserId
        };

        if (task.Id == 0) log.Task = task;

        await _context.AddAsync(log);

        _cache.ClearCaches(CacheKeys.TaskLogsCacheKeys(task.Id));
    }

    #endregion
}