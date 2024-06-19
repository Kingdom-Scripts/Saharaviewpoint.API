using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Auth;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Saharaviewpoint.Core.Services;

public class TokenHandler : ITokenHandler
{
    private readonly JwtConfig _jwtConfig;
    private readonly SaharaviewpointContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenHandler(IOptions<JwtConfig> jwtConfig, SaharaviewpointContext context, IHttpContextAccessor httpContextAccessor)
    {
        _jwtConfig = jwtConfig.Value;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<Result> GenerateJwtToken(User user)
    {
        DateTime expiresAt = DateTime.UtcNow.AddDays(_jwtConfig.Expires);

        // get the request domain
        string? requestDomain = _httpContextAccessor.HttpContext!.Request.Headers["Origin"].ToString();

        string? token = GenerateAccessToken(user, requestDomain, expiresAt);

        // store the token
        var encryptedToken = token.HashPassword();
        var login = await _context.Logins.FirstOrDefaultAsync(l => l.UserId == user.Id);
        if (login != null)
        {
            login.HashedToken = encryptedToken;
            login.ExpiresAt = DateTime.UtcNow.AddDays(30);

            _context.Logins.Update(login);
        } else {
            login = new Login
            {
                UserId = user.Id,
                HashedToken = encryptedToken,
                Domain = requestDomain,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            await _context.Logins.AddAsync(login);
        }
        await _context.SaveChangesAsync();

        var result = new AuthDataView
        {
            User = user.Adapt<UserView>(),
            Token = token,
            RefreshToken = await GenerateRefreshToken(user.Id),
            ExpiresAt = expiresAt
        };

        return new SuccessResult(result);
    }

    public async Task<Result> RefreshJwtToken(string refreshToken)
    {
        var refreshTokenObject = await _context.RefreshTokens
            .Include(r => r.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(r => r.Code == refreshToken);

        if (refreshTokenObject == null || refreshTokenObject.ExpiresAt < DateTime.UtcNow)
            return new ErrorResult("Timeout", "User session expired, kindly log in again.");

        await InvalidateToken(refreshTokenObject.User.Uid.ToString());

        return await GenerateJwtToken(refreshTokenObject.User);
    }

    public async Task InvalidateToken(string userReference)
    {
        var login = await _context.Logins
            .FirstOrDefaultAsync(l => l.User!.Uid.ToString() == userReference);

        if (login != null)
        {
            login.HashedToken = string.Empty;
            login.ExpiresAt = DateTime.UtcNow;

            _context.Logins.Update(login);
            await _context.SaveChangesAsync();
        }

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.User.Uid.ToString() == userReference);

        if (refreshToken != null)
        {
            _context.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ValidateToken(string uid, string token, string domain)
    {
        try
        {
            var today = DateTime.UtcNow;
            var hashedToken = await _context.Logins
                .Where(l => l.User!.Uid.ToString() == uid && l.Domain == domain && l.ExpiresAt > today)
                .Select(l => l.HashedToken)
                .FirstOrDefaultAsync();

            if (hashedToken is null) return false;

            return hashedToken.VerifyPassword(token);
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Error validating token");
            return false;
        }
    }

    private string GenerateAccessToken(User user, string requestDomain, DateTime expiresAt)
    {
        // validate domain
        string[]? domains = _jwtConfig.AllowedDomains.Split(",");
        // TODO: uncomment the code below for live
        // if (!domains.Contains(requestDomain))
        //     throw new Exception("Unable to process request");

        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var claimIdentity = new ClaimsIdentity();

        claimIdentity.AddClaims(new[] { new Claim("uid", user.Uid.ToString()) });
        claimIdentity.AddClaims(new[] { new Claim("sid", user.Id.ToString()) });
        claimIdentity.AddClaims(new []{ new Claim("name", $"{user.FirstName} {user.LastName}") });
        claimIdentity.AddClaims(new[] { new Claim("Type", user.Type) });
        claimIdentity.AddClaims(new[] { new Claim("SubscriptionPlan", "Basic") }); // TODO: use this for subscription plans

        claimIdentity.AddClaims(user.UserRoles.Select(role =>
            new Claim(ClaimTypes.Role, role.Role.Name)));

        byte[]? key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtConfig.Audience,
            Issuer = _jwtConfig.Issuer,
            Subject = claimIdentity,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string? token = tokenHandler.WriteToken(securityToken);

        return token;
    }

    private async Task<string> GenerateRefreshToken(int userId)
    {
        // Create a byte array to store the random bytes
        byte[]? randomNumber = new byte[64];

        // Generate a random characters
        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        string token = Convert.ToBase64String(randomNumber);

        // store the refresh token
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == userId);
        if (refreshToken == null)
        {
            await _context.AddAsync(new RefreshToken
            {
                UserId = userId,
                Code = token,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshExpireDays)
            });
        }
        else
        {
            refreshToken.Code = token;
            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshExpireDays);
        }
        await _context.SaveChangesAsync();

        return token;
    }
}