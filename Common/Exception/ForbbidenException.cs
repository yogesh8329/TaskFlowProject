using System.Net;

namespace TaskFlow.Api.Common.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Forbidden")
            : base(message, (int)HttpStatusCode.Forbidden)
        {
        }
    }
}
