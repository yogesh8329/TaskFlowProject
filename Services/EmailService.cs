
using global::TaskFlow.Api.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;


namespace TaskFlow.Api.Services
{

    public class EmailService : IEmailService
        {
            private readonly EmailSettings _settings;

            public EmailService(IOptions<EmailSettings> settings)
            {
                _settings = settings.Value;
            }

            public async Task SendResetEmailAsync(string toEmail, string resetLink)
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_settings.From),
                    Subject = "Reset Your Password",
                    Body = $"Click the link below to reset your password:\n\n{resetLink}",
                    IsBodyHtml = false
                };

                message.To.Add(toEmail);


            using var client = new SmtpClient(_settings.Host, _settings.Port);
            
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password
            );
            client.EnableSsl = true;

            await client.SendMailAsync(message);
        }
        }
    }