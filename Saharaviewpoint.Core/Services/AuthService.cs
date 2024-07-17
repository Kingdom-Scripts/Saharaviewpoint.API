using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Auth;
using Saharaviewpoint.Core.Utilities;
using System.Web;
using Saharaviewpoint.Models.Configurations;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Contants;

namespace Saharaviewpoint.Core.Services;

// TODO: implement cache for user profile
public class AuthService : IAuthService
{
    private readonly SaharaviewpointContext _context;
    private readonly ITokenHandler _tokenGenerator;
    private readonly UserSession _userSession;
    private readonly IEmailService _emailService;
    private readonly BaseURLs _baseUrls;

    public AuthService(SaharaviewpointContext context, ITokenHandler tokenGenerator, UserSession userSession,
        IEmailService emailService, IOptions<AppConfig> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        _baseUrls = options.Value.BaseURLs;
    }

    public async Task<Result> CreateClient(RegisterModel model)
    {
        // validate user with email doesn't exist
        bool userExist = await _context.Users
            .AnyAsync(u => u.Email == model.Email.ToLower().Trim());

        if (userExist)
            return new ErrorResult("An account with this email already exist. Please log in instead.");

        // create user object
        var user = model.Adapt<User>();
        user.Email = user.Email.ToLower().Trim();
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
        User? user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
            return new BadErrorResult("Invalid credentials.");

        if (!user.IsActive)
            return new BadErrorResult("Account suspended, kindly contact the admin.");

        if (!user.HashedPassword.VerifyPassword(model.Password))
            return new BadErrorResult("Invalid credentials.");

        return await _tokenGenerator.GenerateJwtToken(user);
    }

    public async Task<Result> RefreshToken(RefreshTokenModel model)
    {
        return await _tokenGenerator.RefreshJwtToken(model.RefreshToken);
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

    public async Task<Result> ForgotPassword(ForgotPasswordModel model)
    {
        string responseMessage = "If this email is associated with an account, you will receive a password reset email shortly.";

        // validate user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim());
        if (user is null)
            return new SuccessResult(responseMessage);

        string token = CodeGenerator.GenerateCode(100);

        bool saved = await SaveNewCode(user.Id, token, CodePurposes.ResetPassword);

        if (!saved)
            return new ErrorResult("Unable to send password reset email at the moment. Please try again.");

        // get and encode the url with token
        string url =
            $"{_baseUrls.Client}/auth/reset-password?email={model.Email}&token={HttpUtility.UrlEncode(token)}";

        // send email
        var args = new Dictionary<string, string?> {
            {
                "url", url
            },
            {
                "name", user.FirstName
            },
            {
                "sender_name", _userSession.Name
            }
        };

        var emailRes = await _emailService.SendEmail(model.Email, "Reset Your Password - Saharaviewpoint", EmailTemplates.ForgotPassword, args);
        if (!emailRes.Success)
            return emailRes;

        return new SuccessResult(content: responseMessage);
    }

    public async Task<Result> ResetPassword(ResetPasswordModel model)
    {
        // validate user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim());
        if (user is null)
            return new ErrorResult("Invalid request: User does not exist");

        // validate token
        var today = DateTime.UtcNow;
        var code = await _context.Codes
            .FirstOrDefaultAsync(c => c.OwnerId == user.Id
                && c.Purpose == CodePurposes.ResetPassword
                && c.Token == model.Token
                && c.Used == false);

        if (code == null)
            return new ErrorResult("Invalid request, kindly request a new password reset email.");

        if (code.ExpiryDate < today)
            return new ErrorResult("Password reset token has expired. Kindly request a new one.");

        // update user and token
        user.HashedPassword = model.Password.HashPassword();
        code.Used = true;

        int saved = await _context.SaveChangesAsync();

        var login = await AuthenticateUser(new LoginModel { Email = model.Email, Password = model.Password });

        if (saved < 1)
            return new ErrorResult("Unable to reset password at the moment. Please try again.");

        // send password reset notification Email
        await _emailService.SendEmail(model.Email, "Your Password Was Just Reset - Saharaviewpoint", EmailTemplates.ResetPassword);

        if (!login.Success)
            return new SuccessResult("Password reset successfully, kindly log in to your account again.");

        return new SuccessResult("Password reset successful.", login.Content);
    }

    #region Private Method

    private async Task<bool> SaveNewCode(int userId, string token, string purpose)
    {
        // set all existing code for this user and purpose to used
        var existingCodes = await _context.Codes
            .Where(c => c.OwnerId == userId && c.Purpose == purpose && !c.Used)
            .ToListAsync();

        foreach (var code in existingCodes)
        {
            code.Used = true;
        }

        Code newCode = new()
        {
            OwnerId = userId,
            Token = token,
            Purpose = purpose,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        await _context.AddAsync(newCode);

        int saved = await _context.SaveChangesAsync();
        return saved > 0;
    }

    #endregion
}