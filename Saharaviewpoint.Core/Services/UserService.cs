using Mapster;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Models.Input.User;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Auth;

namespace Saharaviewpoint.Core.Services
{
    public class UserService
    {
        private readonly SaharaviewpointContext _context;

        public UserService(SaharaviewpointContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> ListProjectManagersAsync(int pageIndex, int pageSize)
        {
            var projectManagers = await _context.UserRoles
                .Where(uRole => uRole.RoleId == (int)Roles.SvpManager)
                .Select(uRole => uRole.User)
                .OrderBy(u => u.FirstName)
                .ProjectToType<UserView>()
                .ToPaginatedListAsync(pageIndex, pageSize);

            return new SuccessResult(projectManagers);
        }

        public Result InviteProjectManager(ProjectManagerModel model)
        {
            var mappedInvitation = model.Adapt<PMInvitation>();
            mappedInvitation.ExpiryDate = DateTime.UtcNow.AddDays(7);


        }
    }
}
