using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IAuthService
{
    Task<Result> CreateUserAsync(RegisterModel model);

    Task<Result> AuthenticateUser(LoginModel model);

    Task<Result> RefreshToken(RefreshTokenModel model);

    Task<Result> Logout(string userReference);

    Task<Result> UserProfile();
}