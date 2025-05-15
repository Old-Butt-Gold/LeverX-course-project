using System.Net;

namespace EER.Domain.Exceptions;

public abstract class DomainConflictException : DomainException
{
    public DomainConflictException(string message) : base(message, HttpStatusCode.Conflict) 
    { }
}