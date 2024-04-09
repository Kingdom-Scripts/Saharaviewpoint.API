using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Models.Configurations;
using Saharaviewpoint.Core.Models.Input.User;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Auth;
using Saharaviewpoint.Core.Models.View.User;
using Saharaviewpoint.Core.Utilities;
using System.Text;
using System.Web;
using Saharaviewpoint.Core.Models.Email;
using Saharaviewpoint.Core.Models.Input.Auth;

namespace Saharaviewpoint.Core.Services;

public class UserService : IUserService
{
    private readonly SaharaviewpointContext _context;
    private readonly AppConfig _appConfig;
    private readonly IEmailService _emailService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly UserSession _userSession;

    public UserService(SaharaviewpointContext context, IOptions<AppConfig> appConfig, IEmailService emailService, ITokenGenerator tokenGenerator, UserSession userSession)
    {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appConfig = appConfig.Value;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
            _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        }

    public async Task<Result> ListProjectManagersAsync(string? searchQuery, int pageIndex, int pageSize)
    {
            var projectManagers = await _context.UserRoles
                .Where(uRole => uRole.RoleId == (int)Roles.SvpManager)
                .Where(uRole => string.IsNullOrEmpty(searchQuery)
                                || uRole.User.Email.Contains(searchQuery)
                                || (uRole.User.FirstName!.Contains(searchQuery))
                                || (uRole.User.LastName!.Contains(searchQuery)))
                .SelectMany(uRole => uRole.User.Projects.DefaultIfEmpty(), (userRole, project) => new
                {
                    User = userRole.User,
                    Project = project
                })
                .GroupBy(x => x.User)
                .Select(g => new ProjectManagerView
                {
                    Id = g.Key.Id,
                    Uid = g.Key.Uid,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    Email = g.Key.Email,
                    NoOfProjects = g.Count(p => p.Project != null),
                    IsActive = g.Key.IsActive
                })
                .OrderBy(u => u.FirstName)
                .ToPaginatedListAsync(pageIndex, pageSize);

            return new SuccessResult(projectManagers);
        }

    public async Task<Result> InviteProjectManagerAsync(ProjectManagerModel model)
    {
            // confirm email doesn't exist
            bool emailExist = await _context.Users
                .AnyAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim());

            if (emailExist)
            {
                return new ErrorResult("An account with this email already exist.");
            }

            // TODO: uncomment the check below
            // check if user hasn't been invited before
            bool invitationExist = await _context.PMInvitations
                .AnyAsync(i => i.Email.ToLower().Trim() == model.Email.ToLower().Trim());

            // if (invitationExist)
            //     return new ErrorResult("This email has been invited before.");

            var mappedInvitation = model.Adapt<PMInvitation>();
            mappedInvitation.CreatedById = _userSession.UserId;
            mappedInvitation.ExpiryDate = DateTime.UtcNow.AddDays(7);

            string? token = CodeGenerator.GenerateCode(100);

            var emailSent = await _emailService
                .SendInvitationEmail(new InvitationEmailModel
                {
                    Token = token,
                    UserType = UserTypes.SVP_MANAGER,
                    RecipientName = model.FirstName,
                    RecipientEmail = model.Email,
                    SenderName = _context.Users
                        .Where(u => u.Id == _userSession.UserId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault() ?? "A user of Saharaviewpoint"
                });

            mappedInvitation.EmailSent = emailSent.Success;
            await _context.AddAsync(mappedInvitation);

            var code = new Code
            {
                Email = model.Email, Purpose = UserTypes.SVP_MANAGER,
                Token = token, ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
            await _context.AddAsync(code);

            int saved = await _context.SaveChangesAsync();

            return saved > 0
                ? new SuccessResult("Invitation sent successfully.")
                : new ErrorResult("Failed to invite project manager");
        }

    public async Task<Result> AcceptInvitation(AcceptInvitationModel model)
    {
            var today = DateTime.UtcNow;

            // validate request
            var request = await _context.PMInvitations
                .FirstOrDefaultAsync(i => i.Email == model.Email
                    && (!i.IsFulfilled && i.ExpiryDate > today));

            if (request == null)
                return new ErrorResult("Invalid invitation");

            // validate token
            var token = await _context.Codes
                .FirstOrDefaultAsync(c => c.Purpose == model.Type
                    && c.Token == model.Token
                    && c.ExpiryDate > today
                    && c.Used == false);

            if (token == null)
                return new ErrorResult("Invalid invitation, kindly request a new invitation.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == (int)Roles.SvpManager);

            // create user
            var newUser = new User
            {
                Email = request.Email,
                Type = UserTypes.SVP_MANAGER,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                HashedPassword = model.Password.HashPassword()
            };

            // assign role based on type
            var userRole = new UserRole { User = newUser, Role = role };

            // save details
            token.Used = true;
            request.IsFulfilled = true;
            await _context.AddAsync(newUser);
            await _context.AddAsync(userRole);

            int saved = await _context.SaveChangesAsync();
            if (saved < 1)
                return new ErrorResult("Unable to accept invitation, please try again later.");

            // create user token
            newUser.UserRoles = new List<UserRole>() { userRole };
            var authData = await _tokenGenerator.GenerateJwtToken(newUser);

            // return user token
            if (!authData.Success)
                return new ErrorResult(authData.Message);

            return new SuccessResult($"Welcome, {request.FirstName} {request.LastName}", authData.Content);
        }
}