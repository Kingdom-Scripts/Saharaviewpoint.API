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

namespace Saharaviewpoint.Core.Services
{
    public class UserService : IUserService
    {
        private readonly SaharaviewpointContext _context;
        private readonly AppConfig _appConfig;
        private readonly IEmailService _emailService;
        private readonly ITokenGenerator _tokenGenerator;

        public UserService(SaharaviewpointContext context, IOptions<AppConfig> appConfig, IEmailService emailService, ITokenGenerator tokenGenerator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appConfig = appConfig.Value;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        }

        public async Task<Result> ListProjectManagersAsync(int pageIndex, int pageSize)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("=========================================");
            Console.WriteLine();
            var projectManagers = await _context.UserRoles
                .Where(uRole => uRole.RoleId == (int)Roles.SvpManager)
                .SelectMany(uRole => uRole.User.Projects.DefaultIfEmpty(), (userRole, project) => new
                {
                    User = userRole.User,
                    Project = project
                })
                .GroupBy(x => x.User)
                //.Select(uRole => uRole.User)
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
                .ToListAsync();
                //.ToPaginatedListAsync(pageIndex, pageSize);

            Console.WriteLine();
            Console.WriteLine("=========================================");
            Console.WriteLine("=========================================");

            return new SuccessResult(projectManagers);
        }

        public async Task<Result> InviteProjectManagerAsync(ProjectManagerModel model)
        {
            // confirm email doesn't exist
            var emailExist = await _context.Users
                .AnyAsync(u => u.Email.ToLower().Trim() == model.Email.ToLower().Trim());

            if (emailExist)
            {
                return new ErrorResult("An account with this email already exist.");
            }

            var mappedInvitation = model.Adapt<PMInvitation>();
            mappedInvitation.ExpiryDate = DateTime.UtcNow.AddDays(7);

            await _context.AddAsync(mappedInvitation);

            var token = CodeGenerator.GenerateCode(100);
            var emailBody = ComposePMInvitationEmailBody($"{model.FirstName} {model.LastName}", model.Email, token);

            await _emailService.SendMessage(model.Email, "Saharaviewpoint Project Manager Invitation", emailBody);

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
                    && (i.IsFulfilled == false && i.ExpiryDate < today));

            if (request == null)
                return new ErrorResult("Invalid invitation");

            // validate token
            var token = await _context.Codes
                .FirstOrDefaultAsync(c => c.Purpose == model.Type 
                    &&c.Token == model.Token 
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

        private string ComposePMInvitationEmailBody(string fullName, string email, string token)
        {
            // encode string as URL
            string url = $"{_appConfig.BaseURLs.AdminClient}/auth/accept-invitation?email={email}&type={UserTypes.SVP_MANAGER}&token={token}";
            url = HttpUtility.UrlEncode(url);

            var emailBody = new StringBuilder();
            emailBody.Append($"<p>Dear {fullName},</p>");
            emailBody.Append("<p>You have been invited to join Saharaviewpoint as a Project Manager. Please click the link below to complete your registration.</p>");
            emailBody.Append($"<p><a href=\"{url}\">Register</a></p>");
            emailBody.Append("<p>Thank you,</p>");
            emailBody.Append("<p>Saharaviewpoint Team</p>");

            return emailBody.ToString();
        }
    }
}
