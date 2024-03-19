using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Services
{
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
}
