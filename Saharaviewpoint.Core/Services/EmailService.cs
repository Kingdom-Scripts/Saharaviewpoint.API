using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saharaviewpoint.Core.Interfaces;
using System.Net.Mail;
using System.Text;

namespace Saharaviewpoint.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpClient _smtpClient;

        public EmailService(IHttpClientFactory httpClientFactory, ILogger<EmailService> logger)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClient = httpClientFactory.CreateClient("MailerSend");

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _smtpClient = new SmtpClient("plesk6700.is.cc");
            _smtpClient.Port = 587;
            _smtpClient.Credentials = new System.Net.NetworkCredential("test@kingdomscripts.com", "p6kIv33^4");
            _smtpClient.EnableSsl = false;
        }

        public async Task<bool> SendMessage(string to, string subject, string body, Attachment? attachment = null)
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
                return true;

                var request = new
                {
                    from = new { email = "info@trial-o65qngk8emdgwr12.mlsender.net" },
                    to = new List<object>()
                    {
                        new { email = to }
                    },
                    subject = subject,
                    text = body,
                    html = body
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                    "application/json");
                var response = await _httpClient.PostAsync("email", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return false;
            }
            finally
            {
                mail.Dispose();
            }
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

    public class MailModel
    {
        public MailRecipient from { get; set; }
        public List<MailRecipient> to { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string html { get; set; }
    }

    public class MailRecipient
    {
        public string email { get; set; }
    }
}