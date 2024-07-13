using System.Net.Mail;
using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IEmailService
{
    Task<Result> SendConfirmEmail(string to, string token);
    Task<Result> SendInvitationEmail(InvitationEmailModel model);
    Task<Result> SendEmail(string to, string subject, string template, Dictionary<string, string?>? args = null);
    Task<Result> SendEmail(GenericEmailModel model);
}