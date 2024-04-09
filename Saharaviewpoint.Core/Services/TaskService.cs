using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Input.Task;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View;
using Saharaviewpoint.Core.Models.View.Task;

namespace Saharaviewpoint.Core.Services
{
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
            mappedTask.Attachments = new(); // remove the default empty attachment

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
                .ProjectToType<TaskView>()
                .ToPaginatedListAsync(request.PageIndex, request.PageSize);

            return new SuccessResult(tasks);
        }
    }
}