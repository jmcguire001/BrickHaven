// THIS IS FOR APPSETTINGS.JSON
using System.Net.Mail;
using System.Net;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure;

namespace BrickHaven.Models
{
    public interface ISenderEmail
    {
        Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false);
    }

    public class EmailSender : ISenderEmail
    {
        public Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
        {
            string MailServer = Environment.GetEnvironmentVariable("MailServer");
            string FromEmail = Environment.GetEnvironmentVariable("FromEmail");
            string Password = Environment.GetEnvironmentVariable("Password");
            int MailPort = int.Parse(Environment.GetEnvironmentVariable("MailPort"));

            var client = new SmtpClient(MailServer, MailPort)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage(FromEmail, ToEmail, Subject, Body)
            {
                IsBodyHtml = IsBodyHtml
            };

            return client.SendMailAsync(mailMessage);
        }
    }
}
