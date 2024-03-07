using Mapster;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Project;
using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Core.Utilities;

namespace Saharaviewpoint.Core.Services;

public class ProjectService : IProjectService
{
    private readonly SaharaviewpointContext _context;
    private readonly UserSession _userSession;
    private readonly IFileService _fileService;

    public ProjectService(SaharaviewpointContext context, UserSession userSession, IFileService fileService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        _fileService = fileService;
    }

    #region PROJECTS
    public async Task<Result> CreateProject(ProjectModel model)
    {
        var mappedProject = model.Adapt<Project>();
        mappedProject.Status = ProjectStatuses.REQUESTED;
        mappedProject.CreatedById = _userSession.UserId;

        var type = await _context.ProjectTypes.FirstOrDefaultAsync(t => t.Name == model.Type);
        type ??= new ProjectType { Name = model.Type, CreatedById = _userSession.UserId };
        mappedProject.Type = type;

        // upload design file if it exists
        if (model.Design != null)
        {
            var designUpload = await _fileService.UploadFile(model.Title, model.Design);
            if (!designUpload.Success)
                return new ErrorResult("Unable to upload design file", designUpload.Message);

            mappedProject.DesignId = designUpload.Content.Id;
        }

        await _context.AddAsync(mappedProject);

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult(StatusCodes.Status201Created, mappedProject)
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        project.IsDeleted = true;
        project.DeletedById = _userSession.UserId;
        project.DateDeleted = DateTime.UtcNow;

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult()
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> GetProject(int id)
    {
        var project = await _context.Projects
            .ProjectToType<ProjectDetailView>()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        var mappedProject = project.Adapt<ProjectDetailView>();

        return new SuccessResult(mappedProject);
    }

    public async Task<Result> ListProjects(ProjectSearchModel request)
    {

        var shouldGetAll = string.IsNullOrEmpty(request.SearchQuery)
            && string.IsNullOrEmpty(request.Status)
            && !request.StartDueDate.HasValue
            && !request.EndDueDate.HasValue;

        if (shouldGetAll)
        {
            var allProjects = await _context.Projects
                .Where(prd => !prd.IsDeleted)
                .OrderBy(prd => prd.Order)
                .ProjectToType<ProjectView>()
                .ToPaginatedListAsync(request.PageIndex, request.PageSize);

            return new SuccessResult(allProjects);
        }

        string? searchTerm = !string.IsNullOrEmpty(request.SearchQuery)
            ? request.SearchQuery.Trim().ToLower()
            : null;

        var filteredProjects = await _context.Projects
            .Where(prd => !prd.IsDeleted)
            // search by title, description, or status
            .Where(prd => searchTerm == null || (prd.Title.ToLower().Contains(searchTerm) || (prd.Description == null || prd.Description.ToLower().Contains(searchTerm))))
            .Where(prd => string.IsNullOrEmpty(request.Status) || prd.Status == request.Status)
            // filter by due date
            .Where(prd => !request.StartDueDate.HasValue || prd.DueDate >= request.StartDueDate)
            .Where(prd => !request.EndDueDate.HasValue || prd.DueDate <= request.EndDueDate)
            .Where(prd => !request.PriorityOnly || prd.IsPriority)
            .OrderBy(prd => prd.Order)
            .ProjectToType<ProjectView>()
            .ToPaginatedListAsync(request.PageIndex, request.PageSize);

        return new SuccessResult(filteredProjects);
    }

    public async Task<Result> ReassignProject(int id, ReassignProjectModel model)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        bool? userActive = await _context.Users
            .Where(u => u.Id == model.AssigneeId)
            .Select(u => u.IsActive)
            .FirstOrDefaultAsync();

        if (userActive == null)
            return new BadErrorResult("Invalid user");

        if (!userActive.HasValue)
            return new BadErrorResult("User has been deactivated");

        project.AssigneeId = model.AssigneeId;

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult()
            : new ErrorResult("Unable to save changes, please try again later.");
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

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult()
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> UpdateProjectStatus(int id, ProjectStatusModel model)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return new BadErrorResult("Project does not exist");

        project.Status = model.Status;

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult()
            : new ErrorResult("Unable to save changes, please try again later.");
    }
    #endregion

    #region TYPES
    public async Task<Result> CreateType(TaskModel model)
    {
        var typeExist = await _context.ProjectTypes
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

        var saved = await _context.SaveChangesAsync();

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
            ? null : searchTerm.ToLower().Trim();

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
}