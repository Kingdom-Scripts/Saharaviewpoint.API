using System.Net.Mail;
using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IEmailService
{
    public Result SendMessage(string to, string subject, string body, Attachment? attachment = null);
    Task<Result> SendConfirmEmail(string to, string token);
    Task<Result> SendInvitationEmail(InvitationEmailModel model);
}