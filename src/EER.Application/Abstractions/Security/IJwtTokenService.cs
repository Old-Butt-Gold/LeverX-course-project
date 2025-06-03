using System.Security.Claims;
using EER.Domain.Entities;

namespace EER.Application.Abstractions.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    RefreshToken GenerateRefreshToken(User user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
