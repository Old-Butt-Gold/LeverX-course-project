using System.Net;

namespace EER.Domain.Exceptions;

public sealed class DomainRuleException : DomainException
{
    public DomainRuleException(string message) : base(message, HttpStatusCode.UnprocessableEntity) 
    { }
}