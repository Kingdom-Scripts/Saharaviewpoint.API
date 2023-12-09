using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.Auth;

namespace Saharaviewpoint.Core.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtConfig _jwtConfig;
    private readonly ICacheService _cacheService;

    public TokenGenerator(IOptions<JwtConfig> jwtConfig, ICacheService cacheService)
    {
        _jwtConfig = jwtConfig.Value;
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    // public AuthData GenerateJwtToken(User user)
    // {
    //
    //     return new AuthData();
    // }

    public void InvalidateToken(string userReference)
    {
        _cacheService.RemoveToken(userReference);
    }
}