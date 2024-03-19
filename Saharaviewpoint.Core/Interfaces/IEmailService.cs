using System.Net.Mail;

namespace Saharaviewpoint.Core.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendMessage(string to, string subject, string body, Attachment? attachment = null);
        public bool TestAnother();
    }
}