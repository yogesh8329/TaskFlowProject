using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TaskFlow.Api.Common;
using TaskFlow.Api.Data.DTOs;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    /// <summary>
    /// Handles authentication related operations like register, login and password reset.
    /// </summary>
    /// 



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



        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="dto">User registration details</param>
        /// <returns>Returns success message if registration is successful</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Validation error</response>
        // -------- REGISTER --------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User registered successfully"
            });
        }

        /// <summary>
        /// Authenticates user and returns JWT token.
        /// </summary>
        /// <param name="dto">User login credentials</param>
        /// <returns>JWT access token</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid credentials</response>
        /// 

        //[ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        // -------- LOGIN --------
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Login successful",
                Data = new { Token = token }
            });
        }


        /// <summary>
        /// Returns currently authenticated user details.
        /// </summary>
        /// <response code="200">User details returned</response>
        /// <response code="401">Unauthorized</response>
        // -------- CURRENT USER --------
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID missing");

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User fetched successfully",
                Data = new
                {
                    Id = int.Parse(claim.Value),
                    Email = email
                }
            });
        }



        /// <summary>
        /// Sends password reset link to user's email.
        /// </summary>
        /// <param name="dto">User email</param>
        /// <response code="200">Reset email sent</response>
        /// <response code="400">Invalid email</response>
        // -------- FORGOT PASSWORD --------
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            await _authService.GeneratePasswordResetTokenAsync(dto.Email);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "If email exists, reset link sent."
            });
        }


        /// <summary>
        /// Resets user password using reset token.
        /// </summary>
        /// <param name="dto">Reset password details</param>
        /// <response code="200">Password reset successful</response>
        /// <response code="400">Invalid or expired token</response>
        // -------- RESET PASSWORD --------
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password reset successful"
            });
        }
    }
}