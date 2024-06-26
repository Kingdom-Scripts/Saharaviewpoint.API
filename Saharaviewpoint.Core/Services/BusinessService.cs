using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Services;

public class BusinessService(UserSession userSession, SaharaviewpointContext context)
{
    private readonly SaharaviewpointContext _context = context;
    private readonly UserSession _userSession = userSession;

    public Task<Result> CreateBusiness()
    {
            throw new NotImplementedException();
        }
}