// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Utilities;
using Serilog;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Saharaviewpoint.Core.Services;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly AppConfig _appConfig;
    private readonly SaharaviewpointContext _context;

    public EmailService(IWebHostEnvironment hostingEnvironment,
        IOptions<AppConfig> options,
        SaharaviewpointContext context)
    {
        ArgumentNullException.ThrowIfNull(options);

        _appConfig = options.Value;
        _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));

        // var client = new SmtpClient();
        // client.DeliveryMethod = SmtpDeliveryMethod.Network;
        // client.EnableSsl = true;
        // client.Host = "smtppro.zoho.com";
        // client.Port = 587;
        // client.UseDefaultCredentials = false;
        // client.Credentials = new NetworkCredential("thirdparty@kingdomscripts.com", "hDvhi1?y");
        // client.Send(mail);

        _smtpClient = new SmtpClient
        {
            Host = "smtppro.zoho.com",
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("thirdparty@kingdomscripts.com", "hDvhi1?y"),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
        _context = context;
    }

    public async Task<Result> SendConfirmEmail(string to, string token)
    {
        // get template file
        string templatePath =
            Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplates", "email-verify.html");

        // validate file
        if (!File.Exists(templatePath))
        {
            Log.Error("Email template file not found");
            return new ErrorResult("Email template file not found");
        }

        // read template file as string
        string sourceString = await File.ReadAllTextAsync(templatePath);

        var fluidParser = new FluidParser();
        // return error on failure to parse input
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string? fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // get and encode the url with token
        string url =
            $"{_appConfig.BaseURLs.Client}/auth/confirm-email?email={to}&token={HttpUtility.UrlEncode(token)}";

        // parse template using Fluid
        var context = new TemplateContext
        {
            Options =
            {
                MemberAccessStrategy = new UnsafeMemberAccessStrategy()
            }
        };

        context.Options.Filters.AddFilter("to_comma_separated",
            (input, arguments, ctx) => new StringValue($"{input.ToObjectValue():n}"));
        context.SetValue("url", url);

        // compute output
        string output = await fluidTemplate.RenderAsync(context);

        // send email
        return SendMessage(to, "Confirm Your Email Address", output);
    }

    public async Task<Result> SendInvitationEmail(InvitationEmailModel model)
    {
        // get template file
        string templatePath =
            Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplates", "invitation.html");

        // validate file
        if (!File.Exists(templatePath))
        {
            Log.Error("Email template file not found");
            return new ErrorResult("Email template file not found");
        }

        // read template file as string
        string sourceString = await File.ReadAllTextAsync(templatePath);

        var fluidParser = new FluidParser();
        // return error on failure to parse input
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string? fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // get and encode the url with token
        string baseUrl = model.UserType == UserTypes.SVP_MANAGER
            ? _appConfig.BaseURLs.Admin
            : _appConfig.BaseURLs.Client;

        string url = $"{baseUrl}/auth/accept-invitation" +
                     $"?email={model.RecipientEmail}" +
                     $"&type={HttpUtility.UrlPathEncode(model.UserType)}" +
                     $"&token={model.Token}";

        // parse template using Fluid
        var context = new TemplateContext
        {
            Options =
            {
                MemberAccessStrategy = new UnsafeMemberAccessStrategy()
            }
        };

        context.Options.Filters.AddFilter("to_comma_separated",
            (input, arguments, ctx) => new StringValue($"{input.ToObjectValue():n}"));
        context.SetValue("url", url);
        context.SetValue("name", model.RecipientName);
        context.SetValue("inviteSenderName", model.SenderName);

        // compute output
        string output = await fluidTemplate.RenderAsync(context);

        // send email
        return SendMessage(model.RecipientEmail, "Invitation to Join Team - Saharaviewpoint", output);
    }

    public async Task<Result> SendEmail(string to, string subject, string template,
        Dictionary<string, string?>? args = null)
    {
        // get template file
        string templatePath = Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplates", template);

        // validate file
        if (!File.Exists(templatePath))
        {
            Log.Error("Email template file not found");
            return new ErrorResult("Email template file not found");
        }

        // read template file as string
        string sourceString = await File.ReadAllTextAsync(templatePath);

        var fluidParser = new FluidParser();
        // return error on failure to parse input
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string? fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // parse template using Fluid
        var context = new TemplateContext
        {
            Options = { MemberAccessStrategy = new UnsafeMemberAccessStrategy() }
        };

        context.Options.Filters.AddFilter("to_comma_separated", (input, arguments, ctx)
            => new StringValue($"{input.ToObjectValue():n}"));

        args ??= [];
        foreach (var value in args)
        {
            context.SetValue(value.Key, value.Value ?? string.Empty);
        }

        // compute output
        string output = await fluidTemplate.RenderAsync(context);

        // send email
        return SendMessage(to, subject, output);
    }

    public async Task<Result> SendEmail(GenericEmailModel model)
    {
        // get template file
        string templatePath =
            Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplates", "generic-template.html");

        // validate file
        if (!File.Exists(templatePath))
        {
            Log.Error("Email template file not found");
            return new ErrorResult("Email template file not found");
        }

        // read template file as string
        string sourceString = await File.ReadAllTextAsync(templatePath);

        var fluidParser = new FluidParser();
        // return error on failure to parse input
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string? fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // parse template using Fluid
        var context = new TemplateContext
        {
            Options = { MemberAccessStrategy = new UnsafeMemberAccessStrategy() }
        };

        context.Options.Filters.AddFilter("to_comma_separated", (input, arguments, ctx)
            => new StringValue($"{input.ToObjectValue():n}"));

        // TODO: logo is not showing in received email
        context.SetValue("salutation", model.Salutation);
        context.SetValue("primaryMessage", model.PrimaryMessage);
        context.SetValue("secondaryMessage", model.SecondaryMessage);
        context.SetValue("closingRemark", model.ClosingRemark);
        context.SetValue("showAlternateUrl", model.ShowAlternateUrl);

        if (model.ActionButton is not null)
        {
            context.SetValue("buttonText", model.ActionButton.Text);
            context.SetValue("url", model.ActionButton.Url);
        }

        // compute output
        string output = await fluidTemplate.RenderAsync(context);

        // send email
        return SendMessage(model.To, model.Subject, output, model.Cc, model.Bcc);
    }

    public string GetUserEmails(params string[] roles)
        => string.Join(", ", _context.Roles
                        .Where(r => roles.Contains(r.Name))
                        .SelectMany(r => r.UserRoles)
                        .Select(ur => ur.User!.Email)
                        .ToList());

    private Result SendMessage(string to, string subject, string body, string? cc = null, string? bcc = null,
        Attachment? attachment = null)
    {
        var mail = new MailMessage();

        try
        {
            mail.From = new MailAddress("thirdparty@kingdomscripts.com", "Saharaviewpoint");

            //create Alrternative HTML view
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "svp-logo.png");

            //Add Image
            LinkedResource theEmailImage = new(filePath)
            {
                ContentId = "logoImageID"
            };

            //Add the Image to the Alternate view
            htmlView.LinkedResources.Add(theEmailImage);

            //Add view to the Email Message
            mail.AlternateViews.Add(htmlView);

            mail.To.Add(to);
            if (cc is not null) mail.CC.Add(cc);
            if (bcc is not null) mail.Bcc.Add(bcc);
            mail.Subject = subject;
            mail.IsBodyHtml = true;

            _smtpClient.Send(mail);

            return new SuccessResult(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error sending email to {to}. Subject: {subject}");
            return new ErrorResult(ex.Message);
        }
        finally
        {
            mail.Dispose();
        }
    }
}