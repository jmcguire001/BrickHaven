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
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
        {
            string MailServer = _configuration["EmailSettings:MailServer"];
            string FromEmail = _configuration["EmailSettings:FromEmail"];
            string Password = _configuration["EmailSettings:Password"];
            int MailPort = int.Parse(_configuration["EmailSettings:MailPort"]);


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

//using System.Net;
//using System.Net.Mail;
//using System.Threading.Tasks;
//using Azure;
//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;
//using Microsoft.Extensions.Configuration;

//namespace BrickHaven.Models
//{
//    public interface ISenderEmail
//    {
//        Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false);
//    }

//    public class EmailSender : ISenderEmail
//    {
//        private readonly IConfiguration _configuration;
//        private readonly SecretClient _secretClient;

//        public EmailSender(IConfiguration configuration)
//        {
//            _configuration = configuration;

//            // Initialize the SecretClient using ManagedIdentityCredential
//            var keyVaultUri = new Uri("https://tgplacekeyvault.vault.azure.net/");
//            _secretClient = new SecretClient(keyVaultUri, new ManagedIdentityCredential());
//        }

//        public async Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
//        {
//            string MailServer = (await GetSecretAsync("MailServer")).Value.Value;
//            string FromEmail = (await GetSecretAsync("FromEmail")).Value.Value;
//            string Password = (await GetSecretAsync("Password")).Value.Value;
//            int MailPort = int.Parse((await GetSecretAsync("MailPort")).Value.Value);

//            var client = new SmtpClient(MailServer, MailPort)
//            {
//                Credentials = new NetworkCredential(FromEmail, Password),
//                EnableSsl = true,
//            };

//            MailMessage mailMessage = new MailMessage(FromEmail, ToEmail, Subject, Body)
//            {
//                IsBodyHtml = IsBodyHtml
//            };

//            await client.SendMailAsync(mailMessage);
//        }

//        private async Task<Response<KeyVaultSecret>> GetSecretAsync(string secretName)
//        {
//            return await _secretClient.GetSecretAsync(secretName);
//        }
//    }
//}
