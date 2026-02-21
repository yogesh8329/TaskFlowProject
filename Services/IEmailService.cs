namespace TaskFlow.Api.Services
{
        public interface IEmailService
        {
            Task SendResetEmailAsync(string toEmail, string resetLink);
        }
    }