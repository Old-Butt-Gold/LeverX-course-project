using System.Net;

namespace EER.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected DomainException(string message, HttpStatusCode statusCode) 
        : base(message) 
    {
        StatusCode = statusCode;
    }
}