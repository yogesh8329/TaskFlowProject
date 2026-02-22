using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
namespace TaskFlow.Api.Services
{
    public class EmailService : IEmailService
    {
        public EmailService()
        {
        }

        public async Task SendResetEmailAsync(string toEmail, string resetLink)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("SendGrid API key not configured");

            var client = new SendGridClient(apiKey);

            var fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL")
                            ?? "ymrathod1412@gmail.com";

            var from = new EmailAddress(fromEmail, "TaskFlow");
            var to = new EmailAddress(toEmail);

            var subject = "Reset Your Password";

            var plainTextContent = $"Click the link below to reset your password:\n\n{resetLink}";

            var htmlContent = $@"
                <p>Click the link below to reset your password:</p>
                <p>
                    <a href='{resetLink}' 
                       style='background-color:#2563eb;
                              color:white;
                              padding:10px 15px;
                              text-decoration:none;
                              border-radius:5px;'>
                        Reset Password
                    </a>
                </p>";

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                plainTextContent,
                htmlContent
            );

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid failed: {body}");
            }
        }
    }
}