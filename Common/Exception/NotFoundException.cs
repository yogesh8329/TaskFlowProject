using System.Net;

namespace TaskFlow.Api.Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message = "Resource not found")
            : base(message, (int)HttpStatusCode.NotFound)
        {
        }
    }
}
