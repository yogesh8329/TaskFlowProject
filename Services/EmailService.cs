using SendGrid;
using SendGrid.Helpers.Mail;

namespace TaskFlow.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")?.Trim()
                      ?? throw new Exception("SENDGRID_API_KEY is not configured");

            _fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL")?.Trim()
                         ?? throw new Exception("SENDGRID_FROM_EMAIL is not configured");

            _fromName = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME")?.Trim()
                        ?? "TaskFlow";
        }

        public async Task SendResetEmailAsync(string toEmail, string resetLink)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required");

            if (string.IsNullOrWhiteSpace(resetLink))
                throw new ArgumentException("Reset link is required");

            var client = new SendGridClient(_apiKey);

            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);

            var subject = "Reset Your Password";

            var plainTextContent =
                $"Click the link below to reset your password:\n\n{resetLink}";

            var htmlContent = $@"
                <div style='font-family:Arial,sans-serif'>
                    <h2>Password Reset Request</h2>
                    <p>You requested to reset your password.</p>
                    <p>
                        <a href='{resetLink}'
                           style='background-color:#2563eb;
                                  color:white;
                                  padding:10px 16px;
                                  text-decoration:none;
                                  border-radius:6px;
                                  display:inline-block;'>
                            Reset Password
                        </a>
                    </p>
                    <p>If you did not request this, ignore this email.</p>
                </div>";

            var message = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                plainTextContent,
                htmlContent
            );

            var response = await client.SendEmailAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new Exception(
                    $"SendGrid failed. Status: {response.StatusCode}. Body: {errorBody}"
                );
            }
        }
    }
}