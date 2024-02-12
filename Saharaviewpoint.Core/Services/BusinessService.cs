using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Auth;
using Saharaviewpoint.Core.Utilities;

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
