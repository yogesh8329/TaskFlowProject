using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TaskFlow.Api.Common.Exceptions;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        public AuthService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        // ================= REGISTER =================
        public async Task RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequestException("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password is required");

            var exists = await _context.Users
                .AnyAsync(x => x.Email == dto.Email);

            if (exists)
                throw new ConflictException("User already exists");

            var user = new User
            {
                Email = dto.Email.Trim().ToLower(),
                Role = "User", // 🔒 ALWAYS DEFAULT
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher
                .HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // ================= LOGIN =================
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            // ❌ USER NOT FOUND
            if (user == null)
            {
                //await LogLoginFailed(dto.Email, "User not found");
                throw new UnauthorizedException("Invalid credentials");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, dto.Password);

            // ❌ PASSWORD WRONG
            if (result == PasswordVerificationResult.Failed)
            {
                //await LogLoginFailed(dto.Email, "Invalid password");
                throw new UnauthorizedException("Invalid credentials");
            }

            // ✅ LOGIN SUCCESS
            await LogLoginSuccess(user);

            // ===== JWT =====
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiryMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ================= AUDIT HELPERS =================
        private async Task LogLoginSuccess(User user)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                EntityName = "Auth",
                EntityId = user.Id,
                Action = AuditAction.LoginSuccess,
                OldValues = null,
                NewValues = JsonSerializer.Serialize(new
                {
                    user.Email,
                    user.Role
                }),
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        private async Task LogLoginFailed(string email, string reason)
        {
            _ = _context.AuditLogs.Add(new AuditLog
            {
                UserId = null,
                EntityName = "Auth",
                EntityId = 0,
                Action = AuditAction.LoginFailed,
                OldValues = null,
                NewValues = JsonSerializer.Serialize(new
                {
                    Email = email,
                    Reason = reason
                }),
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email.Trim().ToLower());

            if (user == null)
                return null;

            var token = Guid.NewGuid().ToString();

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:4200/auth/reset-password?token={token}";

            try
            {
                await _emailService.SendResetEmailAsync(user.Email, resetLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMAIL ERROR: " + ex.Message);
            }

            return token;
        }
       
        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new BadRequestException("Invalid token");

            token = token.Trim();

            Console.WriteLine("Incoming Token: " + token);

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.PasswordResetToken == token);

            if (user == null)
                throw new BadRequestException("Invalid token");

            Console.WriteLine("DB Token: " + user.PasswordResetToken);
            Console.WriteLine("Expiry: " + user.PasswordResetTokenExpiry);

            if (user.PasswordResetTokenExpiry == null ||
                user.PasswordResetTokenExpiry <= DateTime.UtcNow)
            {
                throw new BadRequestException("Token expired");
            }

            user.PasswordHash = _passwordHasher
                .HashPassword(user, newPassword);

            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();
        }
    }
}
