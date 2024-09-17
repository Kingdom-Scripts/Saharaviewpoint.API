// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Web;
using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Contants;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Utilities;
using Serilog;

namespace Saharaviewpoint.Core.Services;

public class EmailService : IEmailService
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly SaharaviewpointContext _context;
    private readonly AppConfig _appConfig;
    private readonly ZeptoMailConfig _zeptoMailConfig;
    private readonly HttpClient _zeptoMailClient;
    private readonly string _logoFileKey;

    public EmailService(IWebHostEnvironment hostingEnvironment,
        IOptions<AppConfig> options,
        SaharaviewpointContext context, IOptions<ZeptoMailConfig> zeptoMailConfig, IHttpClientFactory httpClientFactory, ScopedSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentException.ThrowIfNullOrEmpty(nameof(zeptoMailConfig));
        ArgumentException.ThrowIfNullOrEmpty(nameof(httpClientFactory));

        _appConfig = options.Value;
        _zeptoMailConfig = zeptoMailConfig.Value;
        _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        _zeptoMailClient = httpClientFactory.CreateClient(HttpClientKeys.ZeptoMail);
        _context = context;

        _logoFileKey = secrets.ZeptoLogoKey;
    }

    public async Task<Result> SendZeptoMailTemplate(string emailKey, EmailModel model)
    {
        var payload = new
        {
            from = model.From is not null
                ? model.From
                : new EmailAddress { Address = "no-reply@saharaviewpoint.com", Name = "Saharaviewpoint" },
            to = new[] { new { email_address = model.To } },
            template_key = emailKey,
            merge_info = model.MetaData
        };

        var jsonContent = payload.ToJsonContent();

        var response = await _zeptoMailClient.PostAsync("email/template", jsonContent);
        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Error sending email. Status code: {StatusCode}", response.StatusCode);
            return new ErrorResult("Error sending email");
        }

        return new SuccessResult(true);
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
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // get and encode the url with token
        string url =
            $"{_appConfig.BaseUrLs.Client}/auth/confirm-email?email={to}&token={HttpUtility.UrlEncode(token)}";

        // parse template using Fluid
        var context = new TemplateContext
        {
            Options =
            {
                MemberAccessStrategy = new UnsafeMemberAccessStrategy()
            }
        };

        context.Options.Filters.AddFilter("to_comma_separated",
            (input, _, _) => new StringValue($"{input.ToObjectValue():n}"));
        context.SetValue("url", url);

        // compute output
        string output = await fluidTemplate.RenderAsync(context);

        // send email
        // return SendMessage(to, "Confirm Your Email Address", output);
        return await SendMessage(new EmailModel
        {
            To = [new EmailAddress { Address = to }],
            Subject = "Confirm Your Email Address",
            HtmlBody = output
        });
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
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // get and encode the url with token
        string baseUrl = model.UserType == UserTypes.SVP_MANAGER
            ? _appConfig.BaseUrLs.Admin
            : _appConfig.BaseUrLs.Client;

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
            (input, _, _) => new StringValue($"{input.ToObjectValue():n}"));
        context.SetValue("url", url);
        context.SetValue("name", model.RecipientName);
        context.SetValue("inviteSenderName", model.SenderName);

        // compute output
        string output = await fluidTemplate.RenderAsync(context);

        // send email
        // return SendMessage(model.RecipientEmail, "Invitation to Join Team - Saharaviewpoint", output);
        return await SendMessage(new EmailModel
        {
            To = [new EmailAddress { Address = model.RecipientEmail, Name = model.RecipientName }],
            Subject = "Invitation to Join Team - Saharaviewpoint",
            HtmlBody = output
        });
    }

    public async Task<Result> SendEmail(string to, string subject, string template,
        Dictionary<string, string> args = null)
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
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // parse template using Fluid
        var context = new TemplateContext
        {
            Options = { MemberAccessStrategy = new UnsafeMemberAccessStrategy() }
        };

        context.Options.Filters.AddFilter("to_comma_separated", (input, _, _)
            => new StringValue($"{input.ToObjectValue():n}"));

        args ??= [];
        foreach (var value in args)
        {
            context.SetValue(value.Key, value.Value ?? string.Empty);
        }

        // compute output
        string output = await fluidTemplate.RenderAsync(context);

        // send email
        // return SendMessage(to, subject, output);
        return await SendMessage(new EmailModel
        {
            To = [new EmailAddress { Address = to }],
            Subject = subject,
            HtmlBody = output
        });
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
        if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string fluidError))
        {
            Log.Error("Error in parsing template: {FluidError}", fluidError);
            return new ErrorResult($"Error in parsing template: {fluidError}");
        }

        // parse template using Fluid
        var context = new TemplateContext
        {
            Options = { MemberAccessStrategy = new UnsafeMemberAccessStrategy() }
        };

        context.Options.Filters.AddFilter("to_comma_separated", (input, _, _)
            => new StringValue($"{input.ToObjectValue():n}"));

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
        // return SendMessage(model.To, model.Subject, output, model.Cc, model.Bcc);
        return await SendMessage(new EmailModel
        {
            To = model.To,
            Cc = model.Cc,
            Bcc = model.Bcc,
            Subject = model.Subject,
            HtmlBody = output,
            Attachments = model.Attachments
        });
    }

    public List<EmailAddress> GetUserEmails(params string[] roles)
        => _context.Roles
            .Where(r => roles.Contains(r.Name))
            .SelectMany(r => r.UserRoles)
            .Select(ur => new EmailAddress { Address = ur.User.Email, Name = $"{ur.User.FirstName} {ur.User.LastName}" })
            .ToList();

    private async Task<Result> SendMessage(EmailModel model)
    {
        try
        {
            var payload = new
            {
                from = model.From ?? new EmailAddress
                    { Address = _zeptoMailConfig.DefaultSenderAssdress, Name = _zeptoMailConfig.DefaultSenderName },
                to = model.To.Select(cc => new { email_address = cc }).ToArray(),
                cc = model.Cc.Select(cc => new { email_address = cc }).ToArray(),
                bcc = model.Bcc.Select(cc => new { email_address = cc }).ToArray(),
                subject = model.Subject,
                htmlbody = model.HtmlBody,
                merge_info = model.MetaData,
                inline_images = new[]
                {
                    new
                    {
                        file_cache_key = _logoFileKey,
                        cid = "logoImageID"
                    }
                },
                attachments = model.Attachments.Select(file => new
                {
                    name = file.FileName,
                    mime_type = GetFileMimeType(file),
                    content = GetBase64String(file)
                }).ToArray()
            };

            var jsonContent = payload.ToJsonContent();
            var response = await _zeptoMailClient.PostAsync("email", jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Error sending email. Status code: {StatusCode}", response.StatusCode);
                return new ErrorResult("Error sending email");
            }

            return new SuccessResult(true);
        }
        catch (Exception ex)
        {
            // Log.Error(ex, $"Error sending email to {to}. Subject: {subject}");
            Log.Error(ex, "Error sending email to {Addresses}. Subject: {Subject}",
                string.Join(", ", model.To.Select(m => $"{m.Name} <{m.Address}>")), model.Subject);
            return new ErrorResult(ex.Message);
        }
    }

    private string GetFileMimeType(IFormFile file)
    {
        if (file is null)
        {
            return string.Empty;
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(file.FileName, out string contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }

    private string GetBase64String(IFormFile file)
    {
        if (file is null)
        {
            return string.Empty;
        }

        using var ms = new MemoryStream();
        file.CopyTo(ms);
        byte[] fileBytes = ms.ToArray();
        return Convert.ToBase64String(fileBytes);
    }
}