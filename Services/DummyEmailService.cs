namespace TaskFlow.Api.Services
{
    public class DummyEmailService : IEmailService
    {
        public Task SendResetEmailAsync(string toEmail, string resetLink)
        {
           
            return Task.CompletedTask;
        }
    }
}