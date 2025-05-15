using System.Net;

namespace EER.Domain.Exceptions;

public abstract class DomainValidationException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public DomainValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred", HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }
}