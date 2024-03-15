using System.Net.Mail;

namespace Saharaviewpoint.Core.Interfaces
{
    public interface IEmailService
    {
        public bool SendMessage(string from, string to, string subject, string body, Attachment attachment);
    }
}
