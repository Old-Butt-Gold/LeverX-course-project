using System.Net;

namespace EER.Domain.Exceptions;

public abstract class DomainValidationException : DomainException
{
    public DomainValidationException(string message) 
        : base(message, HttpStatusCode.BadRequest) { }
}