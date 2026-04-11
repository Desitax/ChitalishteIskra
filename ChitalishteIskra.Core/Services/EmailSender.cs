using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ChitalishteIskra.Core.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings emailSettings;

        public EmailSender(IOptions<EmailSettings> options)
        {
            emailSettings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var message = new MailMessage();

            // From трябва да е същият като SMTP акаунта
            message.From = new MailAddress(emailSettings.SenderEmail, emailSettings.SenderName);

            message.To.Add(email);
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(emailSettings.SmtpServer, emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(
                    emailSettings.SenderEmail,
                    emailSettings.SenderPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}