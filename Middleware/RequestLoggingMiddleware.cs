using System.Diagnostics;
using System.Security.Claims;

namespace TaskFlow.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                var method = context.Request.Method;
                var path = context.Request.Path;
                var statusCode = context.Response.StatusCode;
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation(
                    "HTTP {Method} {Path} | Status={StatusCode} | UserId={UserId} | Time={Elapsed}ms",
                    method,
                    path,
                    statusCode,
                    userId,
                    elapsedMs
                );
            }
        }
    }
}
