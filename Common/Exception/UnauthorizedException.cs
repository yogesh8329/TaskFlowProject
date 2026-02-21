using System.Net;

namespace TaskFlow.Api.Common.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized")
            : base(message, (int)HttpStatusCode.Unauthorized)
        {
        }
    }
}
