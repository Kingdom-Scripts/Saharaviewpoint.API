using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Services;

public class AuthService : IAuthService
{
    private readonly SaharaviewpointContext _context;

    public AuthService(SaharaviewpointContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Result CreateUser(RegisterModel model)
    {
        throw new NotImplementedException();
    }
}