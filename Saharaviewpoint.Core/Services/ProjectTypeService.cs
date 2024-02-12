using Mapster;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Project;
using Microsoft.EntityFrameworkCore;

namespace Saharaviewpoint.Core.Services;

public class ProjectTypeService : IProjectTypeService
{
    private readonly SaharaviewpointContext _context;
    private readonly UserSession _userSession;

    public ProjectTypeService(SaharaviewpointContext context, UserSession userSession)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
    }

    public async Task<Result> CreateType(string name)
    {
        var typeExist = await _context.ProjectTypes
            .AnyAsync(t => t.Name.ToLower().Trim() ==  name.ToLower().Trim());

        if (typeExist)
            return new ErrorResult("Project type with the name already exist");

        var newType = new ProjectType
        {
            Name = name,
            CreatedById = _userSession.UserId
        };

        await _context.AddAsync(newType);

        var saved = await _context.SaveChangesAsync();

        var savedType = newType.Adapt<ProjectType>();

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

    public async Task<Result> ListTypes() {
        var allTypes = _context.ProjectTypes
            .ProjectToType<ProjectTypeView>();

        return new SuccessResult(allTypes);
    }
}
