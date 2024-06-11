using Saharaviewpoint.Models.Input.User;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IUserService
{
    Task<Result> ListProjectManagersAsync(string? searchQuery, int pageIndex, int pageSize);
    Task<Result> InviteProjectManagerAsync(ProjectManagerModel model);
    Task<Result> AcceptInvitation(AcceptInvitationModel model);
}