using System.Net;

namespace EER.Domain.Exceptions;

public abstract class DomainNotFoundException : DomainException
{
    public DomainNotFoundException(string message) : base(message, HttpStatusCode.NotFound) 
    { }
}