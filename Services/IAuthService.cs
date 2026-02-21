
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task ResetPasswordAsync(string token, string newPassword);

        Task<string?> GeneratePasswordResetTokenAsync(string email);


    }
}
