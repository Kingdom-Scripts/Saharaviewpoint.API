using Saharaviewpoint.Core.Models.Input.User;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IUserService
{
    Task<Result> ListProjectManagersAsync(int pageIndex, int pageSize);
    Task<Result> InviteProjectManagerAsync(ProjectManagerModel model);
    Task<Result> AcceptInvitation(AcceptInvitationModel model);
}