using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saharaviewpoint.Core.Interfaces;
using System.Net.Mail;
using System.Text;
using System.Web;
using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpClient _smtpClient;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly AppConfig _appConfig;

    public EmailService(ILogger<EmailService> logger, IWebHostEnvironment hostingEnvironment,
        IOptions<AppConfig> options)
    {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _appConfig = options.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));

            _smtpClient = new SmtpClient("plesk6700.is.cc");
            _smtpClient.Port = 587;
            _smtpClient.Credentials = new System.Net.NetworkCredential("test@kingdomscripts.com", "p6kIv33^4");
            _smtpClient.EnableSsl = false;
        }

    public Result SendMessage(string to, string subject, string body, Attachment? attachment = null)
    {
            var mail = new MailMessage();
            try
            {
                mail.From = new MailAddress("test@kingdomscripts.com");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                _smtpClient.Send(mail);
                return new SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return new ErrorResult(ex.Message);
            }
            finally
            {
                mail.Dispose();
            }
        }

    public async Task<Result> SendConfirmEmail(string to, string token)
    {
            // get template file
            string templatePath =
                Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplates", "email-verify.html");

            // validate file
            if (!File.Exists(templatePath))
            {
                _logger.LogError("Email template file not found");
                return new ErrorResult("Email template file not found");
            }

            // read template file as string
            string sourceString = await File.ReadAllTextAsync(templatePath);

            var fluidParser = new FluidParser();
            // return error on failure to parse input
            if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string? fluidError))
            {
                _logger.LogError("Error in parsing template: {FluidError}", fluidError);
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
                _logger.LogError("Email template file not found");
                return new ErrorResult("Email template file not found");
            }

            // read template file as string
            string sourceString = await File.ReadAllTextAsync(templatePath);

            var fluidParser = new FluidParser();
            // return error on failure to parse input
            if (!fluidParser.TryParse(sourceString, out var fluidTemplate, out string? fluidError))
            {
                _logger.LogError("Error in parsing template: {FluidError}", fluidError);
                return new ErrorResult($"Error in parsing template: {fluidError}");
            }

            // get and encode the url with token
            string baseUrl = model.UserType == UserTypes.SVP_MANAGER
                ? _appConfig.BaseURLs.Admin
                : _appConfig.BaseURLs.Client;

            string url = $"{baseUrl}/auth/accept-invitation" +
                         $"/{model.RecipientEmail}" +
                         $"/{HttpUtility.UrlPathEncode(model.UserType)}" +
                         $"/{model.Token}";

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


    public bool TestAnother()
    {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("plesk6700.is.cc");

                mail.From = new MailAddress(
                    "test@kingdomscripts.com"); //you have to provide your gmail address as from address
                mail.To.Add("mordecai@kingdomscripts.com");
                mail.Subject = "Test Subject";
                mail.Body = "Test Email Body";

                SmtpServer.Port = 587;
                SmtpServer.Credentials =
                    new System.Net.NetworkCredential("test@kingdomscripts.com",
                        "p6kIv33^4"); //you have to provide you gamil username and password
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
}