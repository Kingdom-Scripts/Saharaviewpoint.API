using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Models.Configurations;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Saharaviewpoint.Core.Services
{
    public class EmailService
    {
        private readonly SmtpClient _client;

        public EmailService(IOptions<KeyVaultConfig> keyVaultConfig)
        {
            if (keyVaultConfig == null) throw new ArgumentNullException(nameof(keyVaultConfig));

            //TODO: Store username and password in keyvault
            //var keyVault = keyVaultConfig.Value;
            //var credential = new ClientSecretCredential(keyVault.DirectoryID, keyVault.ClientId, keyVault.ClientSecret);

            //var client = new SecretClient(new Uri(keyVault.KeyVaultURL), credential);



            _client = new SmtpClient
            {
                Host = "mail.kingdomscripts.com",
                Port = 80,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            _client.UseDefaultCredentials = false;
            _client.Credentials = new NetworkCredential(
                    "mordecai@kingdomscripts.com",
                    "Davidire0)("
                );
        }

        public bool SendMessage(string from, string to, string subject, string body, Attachment attachment)
        {
            MailMessage mail = null;
            var isSent = false;

            try
            {
                //Create message
                mail = new MailMessage(from, to, subject, body)
                {
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                    IsBodyHtml = true,
                    BodyEncoding = UTF8Encoding.UTF8
                };

                // Add attachment
                if (attachment != null)
                    mail.Attachments.Add(attachment);

                //This method send our mail
                _client.Send(mail);
                isSent = true;

            }
            finally
            {
                mail.Dispose();
            }

            return isSent;
        }
    }
}
