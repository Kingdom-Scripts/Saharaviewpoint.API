using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Services;

public class BusinessService
{
    private readonly SaharaviewpointContext _context;
    private readonly UserSession _userSession;

    public BusinessService(UserSession userSession, SaharaviewpointContext context)
    {
            _userSession = userSession;
            _context = context;
        }

    public Task<Result> CreateBusiness()
    {
            throw new NotImplementedException();
        }
}