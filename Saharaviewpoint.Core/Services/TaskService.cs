using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Input.Task;
using Saharaviewpoint.Core.Models.Utilities;
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

            // upload attachments if any
            string folder = project.FolderNames.First();
            string subFolder = project.FolderNames.Last();
            foreach (var file in model.Attachments)
            {
                var uploaded = await _fileService.UploadFileInternal(folder, subFolder, file);
                if (uploaded.Success)
                {
                    mappedTask.Attachments.Add(new TaskAttachment()
                    {
                        Document = uploaded.Content
                    });
                }
                else
                {
                    _logger.LogError(uploaded.Message);
                }
            }

            await _context.AddAsync(mappedTask);

            int saved = await _context.SaveChangesAsync();

            return saved > 0
                ? new SuccessResult(StatusCodes.Status201Created, mappedTask.Adapt<TaskDetailView>())
                : new ErrorResult("Unable to save changes, please try again later.");
        }
    }
}