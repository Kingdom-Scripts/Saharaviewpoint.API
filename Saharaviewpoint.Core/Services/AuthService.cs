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

    public async Task<Result> CreateClient(RegisterModel model)
    {
        // validate user with email doesn't exist
        var userExist = await _context.Users
            .AnyAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim());

        if (userExist)
            return new ErrorResult("An account with this email already exist. Please log in instead.");

        // create user object
        var user = model.Adapt<User>();
        user.Type = UserTypes.CLIENT;
        user.HashedPassword = model.Password.HashPassword();

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == (int)Roles.Client);
        var userRole = new UserRole { User = user, Role = role };

        // save user
        await _context.AddAsync(user);
        await _context.AddAsync(userRole);

        int saved = await _context.SaveChangesAsync();
        if (saved < 1)
            return new ErrorResult("Unable to add user at the moment. Please try again");

        // create user token
        user.UserRoles = new List<UserRole>() { userRole };
        var authData = await _tokenGenerator.GenerateJwtToken(user);

        // return user token
        if (!authData.Success)
            return new ErrorResult(authData.Message);

        return new SuccessResult(StatusCodes.Status201Created, authData.Content);
    }

    public async Task<Result> AuthenticateUser(LoginModel model)
    {
        model.Email = model.Email.ToLower().Trim();
        User user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email);

        if (user == null)
            return new ErrorResult("Login Failed:", "Invalid credentials.");

        if (!user.IsActive)
            return new ErrorResult("Login Failed:", "Account suspended, kindly contact the admin.");

        if (!user.HashedPassword.VerifyPassword(model.Password))
            return new ErrorResult("Login Failed:", "Invalid credentials.");

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