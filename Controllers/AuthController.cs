using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TaskFlow.Api.Common;
using TaskFlow.Api.Common.Exceptions;
using TaskFlow.Api.Data.DTOs;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet("crash-test")]
        public IActionResult CrashTest()
        {
            throw new Exception("Test crash");
        }
        // ---------------- REGISTER ----------------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User registered successfully",
                Data = null
            });
        }

        // ---------------- LOGIN ----------------
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new { Token = token }
                });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Data = null
                });
            }
        }

        // ---------------- CURRENT USER ----------------
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User fetched successfully",
                Data = new
                {
                    Id = userId,
                    Email = email
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var token = await _authService.GeneratePasswordResetTokenAsync(dto.Email);
            return Ok(new { message = "If email exists, reset link sent." });

        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
            return Ok(new { message = "Password reset successful" });
        }

    }
}
