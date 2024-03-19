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

        public EmailService(IHttpClientFactory httpClientFactory, ILogger<EmailService> logger)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClient = httpClientFactory.CreateClient("MailerSend");

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendMessage(string to, string subject, string body, Attachment? attachment = null)
        {
            try
            {
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

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("email", content);

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return false;
            }
            finally
            {
                //mail.Dispose();
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
