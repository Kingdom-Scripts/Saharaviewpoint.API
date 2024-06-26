using Saharaviewpoint.Models.Input.User;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IUserService
{
    Task<Result> ListProjectManagersAsync(string? searchQuery, int pageIndex, int pageSize);
    Task<Result> InviteProjectManagerAsync(ProjectManagerModel model);
    Task<Result> AcceptInvitation(AcceptInvitationModel model);
    Task<Result> SuspendUser(string userUid);
    Task<Result> ActivateUser(string userUid);
    Task<Result> CheckIfEmailExist(string email);
}