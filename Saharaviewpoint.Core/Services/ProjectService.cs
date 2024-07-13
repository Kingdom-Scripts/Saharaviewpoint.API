using Mapster;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Project;
using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.Constants;
using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.View.Task;
using Saharaviewpoint.Models.App.Constants;
using LazyCache;

namespace Saharaviewpoint.Core.Services;

public class ProjectService(SaharaviewpointContext context, UserSession userSession, IFileService fileService,
    IEmailService emailService, IAppCache cache) : IProjectService
{
    private readonly SaharaviewpointContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserSession _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
    private readonly IFileService _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly IAppCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    private const string ListProjectsCacheKeys = "ProjectService-ListProjects-CacheKeys";
    private const string ProjectLogsCacheKeys = "ProjectLogs-CacheKeys";

    #region PROJECTS

    public async Task<Result> CreateProject(ProjectModel model)
    {
        // validate user doesn't have a project with same title
        bool projectExist = await _context.Projects
            .AnyAsync(p => p.Title.ToLower().Trim() == model.Title.ToLower().Trim()
                           && p.CreatedById == _userSession.UserId);
        if (projectExist)
            return new ErrorResult("Project with the title already exist");

        var mappedProject = model.Adapt<Project>();
        mappedProject.Status = ProjectStatuses.Requested;
        mappedProject.CreatedById = _userSession.UserId;

        // get user uid as folder
        string folder = _userSession.Uid.ToLower();
        string subFolder = model.Title.ToFolderName();

        mappedProject.FolderNames.Add(folder);
        mappedProject.FolderNames.Add(subFolder);

        var type = await _context.ProjectTypes.FirstOrDefaultAsync(t => t.Name == model.Type);
        type ??= new ProjectType { Name = model.Type, CreatedById = _userSession.UserId };
        mappedProject.Type = type;

        // upload design file if it exists
        if (model.Design != null)
        {
            var designUpload = await _fileService
                .UploadFileInternal(folder, subFolder, model.Design);
            if (!designUpload.Success)
                return new ErrorResult("Unable to upload design file", designUpload.Message);

            mappedProject.Design = designUpload.Content;
        }

        // add log
        AddProjectLog(mappedProject, ProjectLogTypes.Create, $"Project created {_userSession.Name}");

        // save project to databse
        await _context.AddAsync(mappedProject);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.Remove(ListProjectsCacheKeys);;

        return new SuccessResult(StatusCodes.Status201Created, mappedProject);
    }

    public async Task<Result> ApproveProject(int id, string assigneeUid)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        var assignee = await _context.Users
            .FirstOrDefaultAsync(u => u.Uid.ToString() == assigneeUid);

        if (assignee == null)
            return new BadErrorResult("Assignee does not exist");

        project.AssigneeId = assignee.Id;
        project.Status = ProjectStatuses.InProgress;
        project.UpdatedById = _userSession.UserId;
        project.UpdatedOn = DateTime.UtcNow;

        // add log
        AddProjectLog(project, ProjectLogTypes.Assignment, $"Project assigned to {assignee.FirstName} {assignee.LastName}");

        // save changes to the database
        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.Remove(ListProjectsCacheKeys);;
        _cache.Remove($"project-details-{id}");

        return new SuccessResult();
    }

    public async Task<Result> RejectProject(int id, RejectProjectModel model)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        project.Status = ProjectStatuses.Rejected;
        project.RejectionReason = model.Reason;  // TODO: test this
        project.UpdatedById = _userSession.UserId;
        project.UpdatedOn = DateTime.UtcNow;

        // add log
        AddProjectLog(project, ProjectLogTypes.StatusChange, $"Project rejected by {_userSession.Name}", remark: model.Reason);

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.Remove(ListProjectsCacheKeys);;
        _cache.Remove($"project-details-{id}");

        return new SuccessResult();
    }

    public async Task<Result> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        // user can only delete their own project
        if (project.CreatedById != _userSession.UserId)
            return new ErrorResult(StatusCodes.Status403Forbidden, "You are not authorized to delete this project.");

        project.IsDeleted = true;
        project.DeletedById = _userSession.UserId;
        project.DateDeleted = DateTime.UtcNow;

        // add log
        AddProjectLog(project, ProjectLogTypes.Delete, $"Project deleted by {_userSession.Name}");

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.Remove(ListProjectsCacheKeys);;
        _cache.Remove($"project-details-{id}");

        return new SuccessResult();
    }

    public async Task<Result> GetProject(int id)
    {
        var project = await _cache.GetOrAddAsync($"project-details-{id}", () => _context.Projects
            .ProjectToType<ProjectDetailView>()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted), TimeSpan.FromHours(2));

        if (project == null)
            return new BadErrorResult("Project does not exist");

        // return forbidden if it's client and not the owner
        if (_userSession.AppType == AppTypes.Client
            && project.CreatedById != _userSession.UserId)
            return new ForbiddenResult();

        var mappedProject = project.Adapt<ProjectDetailView>();

        return new SuccessResult(mappedProject);
    }

    public async Task<Result> ListProjects(ProjectSearchModel request)
    {
        // Define a unique cache key based on the request parameters
        var properties = new List<string?>
        {
            request.SearchQuery,
            request.Status,
            request.StartDueDate?.ToString("o"), // Using a round-trip date/time pattern
            request.EndDueDate?.ToString("o"),
            request.PageIndex.ToString(),
            request.PageSize.ToString(),
            _userSession.UserId.ToString(), // Include user session details if they affect the result
            _userSession.FilterByAnyClient.ToString(),
            _userSession.FilterByBusinessAdmin.ToString(),
            _userSession.FilterBySvpManager.ToString(),
            request.PriorityOnly.ToString()
        };

        var cacheKey = string.Join("-", properties.Where(p => p != null));

        // Retrieve the current list of cache keys and add the new key
        var cacheKeys = _cache.GetOrAdd(ListProjectsCacheKeys, () => new List<string>(), TimeSpan.FromHours(2));
        if (!cacheKeys.Contains(cacheKey))
        {
            cacheKeys.Add(cacheKey);
            _cache.Add(ListProjectsCacheKeys, cacheKeys);
        }

        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            bool shouldGetAll = string.IsNullOrEmpty(request.SearchQuery)
                            && string.IsNullOrEmpty(request.Status)
                            && !request.StartDueDate.HasValue
                            && !request.EndDueDate.HasValue;

            var projectsQuery = _context.Projects
                .Where(prd => !prd.IsDeleted)
                .AsQueryable();

            // Apply filters based on the user session
            if (_userSession.FilterByAnyClient)
                projectsQuery = projectsQuery.Where(prd => prd.CreatedById == _userSession.UserId);
            if (_userSession.FilterByBusinessAdmin)
                projectsQuery = projectsQuery.Where(prd => prd.CreatedById == _userSession.UserId);
            if (_userSession.FilterBySvpManager)
                projectsQuery = projectsQuery.Where(prd => prd.AssigneeId == _userSession.UserId);

            if (shouldGetAll)
            {
                return await projectsQuery
                    .OrderBy(prd => prd.Order)
                    .ThenByDescending(prd => prd.StartDate)
                    .ProjectToType<ProjectView>()
                    .ToPaginatedListAsync(request.PageIndex, request.PageSize);
            }

            string? searchTerm = !string.IsNullOrEmpty(request.SearchQuery)
                ? request.SearchQuery.Trim().ToLower()
                : null;

            return await projectsQuery
                // search by title, description, or status
                .Where(prd => searchTerm == null || (prd.Title.ToLower().Contains(searchTerm) ||
                                                     (prd.Description == null ||
                                                      prd.Description.ToLower().Contains(searchTerm))))
                .Where(prd => string.IsNullOrEmpty(request.Status) || prd.Status == request.Status)
                // filter by due date
                .Where(prd => !request.StartDueDate.HasValue || prd.DueDate >= request.StartDueDate)
                .Where(prd => !request.EndDueDate.HasValue || prd.DueDate <= request.EndDueDate)
                .Where(prd => !request.PriorityOnly || prd.IsPriority)
                .OrderBy(prd => prd.Order)
                .ProjectToType<ProjectView>()
                .ToPaginatedListAsync(request.PageIndex, request.PageSize);
        }, TimeSpan.FromHours(2));

        return new SuccessResult(cachedResult);
    }

    public async Task<Result> CountProjects()
    {
        int count = await _context.Projects
            .Where(prd => !prd.IsDeleted)
            .CountAsync();

        return new SuccessResult(count);
    }

    public async Task<Result> ReassignProject(int id, ReassignProjectModel model)
    {
        var project = await _context.Projects.Where(p => p.Id == id && !p.IsDeleted)
            .Include(p => p.Assignee)
            .FirstOrDefaultAsync();

        if (project == null)
            return new BadErrorResult("Project does not exist");

        var newAssignee = await _context.Users
            .Where(u => u.Id == model.AssigneeId)
            .Select(u => new
            {
                u.FirstName,
                u.LastName,
                u.IsActive
            })
            .FirstOrDefaultAsync();

        if (newAssignee == null)
            return new BadErrorResult("Invalid user");

        if (!newAssignee.IsActive)
            return new BadErrorResult("User has been deactivated");

        project.AssigneeId = model.AssigneeId;

        // add log
        AddProjectLog(project, ProjectLogTypes.Assignment, $"Project reassigned to {model.AssigneeId}", previousState: $"{project.Assignee!.FirstName} {project.Assignee.LastName}", currentState: $"{newAssignee.FirstName} {newAssignee.LastName}");

        // save changes to the database
        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.Remove(ListProjectsCacheKeys);;
        _cache.Remove($"project-details-{id}");

        return new SuccessResult();
    }

    public async Task<Result> UpdateProject(int id, ProjectModel model)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        if (model.AssigneeId.HasValue)
        {
            bool? userActive = await _context.Users
                .Where(u => u.Id == model.AssigneeId)
                .Select(u => u.IsActive)
                .FirstOrDefaultAsync();

            if (userActive == null)
                return new BadErrorResult("Invalid assignee");

            if (!userActive.HasValue)
                return new BadErrorResult("Assignee has been deactivated from the syste");
        }

        model.Adapt(project);

        project.UpdatedById = _userSession.UserId;
        project.UpdatedOn = DateTime.UtcNow;

        // add log
        AddProjectLog(project, ProjectLogTypes.Update, $"Project updated by {_userSession.Name}");

        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.Remove(ListProjectsCacheKeys);;
        _cache.Remove($"project-details-{id}");

        return new SuccessResult();
    }

    public async Task<Result> UpdateProjectStatus(int id, ProjectStatusModel model)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        string previousStatus = project.Status;
        project.Status = model.Status;

        // add log
        AddProjectLog(project, ProjectLogTypes.StatusChange, $"Project status changed by {_userSession.Name}", previousState: previousStatus, currentState: model.Status);

        // save changes to the database
        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.Remove(ListProjectsCacheKeys);;
        _cache.Remove($"project-details-{id}");

        return new SuccessResult();
    }

    public async Task<Result> ListProjectLogs(int id, PagingOptionModel request)
    {
        // Define a unique cache key based on request parameters
        var properties = new List<string?>
        {
            id.ToString(),
            request.PageIndex.ToString(),
            request.PageSize.ToString()
        };

        var cacheKey = string.Join("-", properties.Where(p => p != null));

        // Retrieve the current list of cache keys and add the new key
        var cacheKeys = _cache.GetOrAdd(ProjectLogsCacheKeys, () => new List<string>(), TimeSpan.FromHours(2));
        if (!cacheKeys.Contains(cacheKey))
        {
            cacheKeys.Add(cacheKey);
            _cache.Add(ProjectLogsCacheKeys, cacheKeys);
        }

        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            return await _context.TaskLogs
                .Where(tl => tl.Task!.ProjectId == id)
                .OrderByDescending(tl => tl.CreatedAt)
                .ProjectToType<PtojectLogView>()
                .ToPaginatedListAsync(request.PageIndex, request.PageSize);
        }, TimeSpan.FromHours(2));

        return new SuccessResult(cachedResult);
    }

    #endregion

    #region TYPES

    public async Task<Result> CreateType(ProjectTypeModel model)
    {
        bool typeExist = await _context.ProjectTypes
            .AnyAsync(t => t.Name.ToLower().Trim() == model.Name.ToLower().Trim()
                           && t.CreatedById == _userSession.UserId);

        if (typeExist)
            return new ErrorResult("Project type with the name already exist");

        var newType = new ProjectType
        {
            Name = model.Name.Trim(),
            CreatedById = _userSession.UserId
        };

        await _context.AddAsync(newType);

        int saved = await _context.SaveChangesAsync();

        var savedType = newType.Adapt<ProjectTypeView>();

        return saved > 0
            ? new SuccessResult(StatusCodes.Status201Created, savedType)
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> DeleteType(int id)
    {
        var type = await _context.ProjectTypes.FindAsync(id);

        if (type == null)
            return new BadErrorResult("Type does not exist.");

        type.IsDeleted = true;
        type.DeletedById = _userSession.UserId;
        type.CreatedAt = DateTime.UtcNow;

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult()
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> ListTypes(string? searchTerm)
    {
        searchTerm = string.IsNullOrEmpty(searchTerm)
            ? null
            : searchTerm.ToLower().Trim();

        var allTypes = await _context.ProjectTypes
            .Where(pt => !pt.IsDeleted)
            .Where(pt => searchTerm == null || pt.Name.ToLower().Trim().Contains(searchTerm))
            .OrderBy(pt => pt.Name)
            .Take(5)
            .ProjectToType<ProjectTypeView>()
            .ToListAsync();

        return new SuccessResult(allTypes);
    }

    #endregion

    #region PRIVATE METHODS

    private async void AddProjectLog(Project project, ProjectLogTypes type, string description, string? previousState = null, string? currentState = null, string? remark = null)
    {
        var log = new ProjectLog
        {
            ProjectId = project.Id,
            Type = type.ToString(),
            Description = description,
            Remark = remark,
            PreviousState = previousState,
            CurrentState = currentState,
            CreatedById = _userSession.UserId
        };

        if (project.Id == 0) log.Project = project;

        await _context.ProjectLogs.AddAsync(log);

        _cache.Remove(ProjectLogsCacheKeys);
    }

    #endregion
}