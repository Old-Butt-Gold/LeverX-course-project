using EER.Domain.Entities;

namespace EER.Application.Abstractions.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
}
