// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IEmailService
{
    Task<Result> SendZeptoMailTemplate(string emailKey, EmailModel model);
    Task<Result> SendConfirmEmail(string to, string token);
    Task<Result> SendInvitationEmail(InvitationEmailModel model);
    Task<Result> SendEmail(string to, string subject, string template, Dictionary<string, string> args = null);
    Task<Result> SendEmail(GenericEmailModel model);
    List<EmailAddress> GetUserEmails(params string[] roles);
}