using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Auth;
using Saharaviewpoint.Core.Utilities;

namespace Saharaviewpoint.Core.Services;

public class AuthService : IAuthService
{
    private readonly SaharaviewpointContext _context;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly UserSession _userSession;

    public AuthService(SaharaviewpointContext context, ITokenGenerator tokenGenerator, UserSession userSession)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
    }

    public async Task<Result> CreateUserAsync(RegisterModel model)
    {
        // validate user with email doesn't exist
        var userExist = await _context.Users
            .AnyAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim());

        if (userExist)
            return new ErrorResult("An account with this email already exist. Please log in instead.");

        // validate user with username doesn't exist
        var userNameExist = await _context.Users
            .AnyAsync(u => u.Username.ToLower().Trim() == model.Username.ToLower().Trim());

        if (userNameExist)
            return new ErrorResult("An account with this username already exist. Please log in instead.");

        // create user object
        var user = new User
        {
            Username = model.Username.Trim(),
            Email = model.Email.Trim(),
            Type = model.Type,
            HashedPassword = model.Password.HashPassword(),
            LastLoginDate = DateTime.UtcNow,
            UserRoles = new List<UserRole>()
        };

        // save user
        await _context.AddAsync(user);
        int saved = await _context.SaveChangesAsync();
        if (saved < 1)
            return new ErrorResult("Unable to add user at the moment. Please try again");

        // create user token
        var authData = await _tokenGenerator.GenerateJwtToken(user);

        // return user token
        if (!authData.Success)
            return new ErrorResult(authData.Message);

        return new SuccessResult(StatusCodes.Status201Created, authData.Content);
    }

    public async Task<Result> AuthenticateUser(LoginModel model)
    {
        model.Username = model.Username.ToLower().Trim();
        User user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username
                || u.Email.ToLower() == model.Username);

        if (user == null)
            return new ErrorResult("Login Failed:", "Username does not exist.");

        if (!user.IsActive)
            return new ErrorResult("Login Failed:", "Account suspended, kindly contact the admin");

        if (!user.HashedPassword.VerifyPassword(model.Password))
            return new ErrorResult("Login Failed:", "Invalid username or password");

        return await _tokenGenerator.GenerateJwtToken(user);
    }

    public async Task<Result> RefreshToken(RefreshTokenModel model)
    {
        return await _tokenGenerator.RefreshJwtToken(model.Token);
    }

    public async Task<Result> Logout(string userReference)
    {
        await _tokenGenerator.InvalidateToken(userReference);
        return new SuccessResult();
    }

    public async Task<Result> UserProfile()
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _userSession.UserId);

        if (user == null)
            return new ErrorResult(StatusCodes.Status404NotFound, "User not found.");

        var userView = user.Adapt<UserProfileView>();

        return new SuccessResult(userView);
    }
}