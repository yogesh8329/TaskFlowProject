using System.Net;

namespace TaskFlow.Api.Common.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message = "Bad request")
            : base(message, (int)HttpStatusCode.BadRequest)
        {
        }
    }
}
