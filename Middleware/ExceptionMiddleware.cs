using System.Net;
using System.Text.Json;
using FluentValidation;
using TaskFlow.Api.Common;
using TaskFlow.Api.Common.Exceptions;

namespace TaskFlow.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;


        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            // ✅ FluentValidation → 400
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = 400,
                    Errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
            // ✅ Custom App Exceptions → dynamic status
            catch (AppException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                _logger.LogError(ex, "Unhandled exception occurred");
                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Message = ex.Message
                });
            }
            // ❌ Unknown errors → 500
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";


                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "Something went wrong"
                });
            }
        }
    }
}
