using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Input.Client;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Client;

namespace Saharaviewpoint.Core.Services;

public class ClientService(SaharaviewpointContext context, UserSession userSession) : IClientService
{
    private readonly SaharaviewpointContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserSession _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));

    public async Task<Result> ListClients(ClientSearchModel request)
    {
        var clients = await _context.Users
            .Where(u => u.Type == UserTypes.CLIENT)
            .Where(u => string.IsNullOrEmpty(request.SearchQuery) || u.FirstName.Contains(request.SearchQuery) || u.LastName.Contains(request.SearchQuery) || u.Email.Contains(request.SearchQuery))
            .Where(u => !request.DateJoinedStart.HasValue || u.CreatedAt >= request.DateJoinedStart.Value.ToUniversalTime())
            .Where(u => !request.DateJoinedEnd.HasValue || u.CreatedAt <= request.DateJoinedEnd.Value.ToUniversalTime())
            .Where(u => !request.IsActiveOnly || u.IsActive == true)
            .Where(u => !request.IsInactiveOnly || u.IsActive == false)
            .OrderBy(u => u.FirstName)
            .Select(u => new ClientView
            {
                Uid = u.Uid,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                //NoOfProjects = u.Projects.Where(p => p.CreatedById == u.Id).Count(),
                NoOfProjects = _context.Projects.Where(p => p.CreatedById == u.Id).Count(),
                IsActive = u.IsActive,
                JoinedOn = u.CreatedAt
            })
            .ToPaginatedListAsync(request.PageIndex, request.PageSize);

        return new SuccessResult(clients);
    }
}
