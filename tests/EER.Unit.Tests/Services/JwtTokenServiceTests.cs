using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EER.Application.Services.Security;
using EER.Application.Settings;
using EER.Domain.Entities;
using EER.Domain.Enums;
using Microsoft.Extensions.Options;

namespace EER.Unit.Tests.Services;

public class JwtTokenServiceTests
{
    private readonly User _user = new()
    {
        Id = Guid.NewGuid(),
        Email = "test@example.com",
        PasswordHash = "passwordHash123!",
        UserRole = Role.Admin
    };

    private readonly JwtSettings _jwtSettings = new()
    {
        Issuer = "test_issuer",
        Audience = "test_audience",
        SigningKey = "this_is_a_very_long_and_secure_key",
        ExpirySeconds = 300,
        RefreshExpirySeconds = 3600,
    };

    [Fact]
    public void GenerateAccessToken_ShouldIncludeCorrectClaims()
    {
        // Arrange
        var service = new JwtTokenService(Options.Create(_jwtSettings));

        // Act
        var token = service.GenerateAccessToken(_user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.Equal(_user.Id.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value);
        Assert.Equal(_user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal(_user.UserRole.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal(_jwtSettings.Issuer, jwtToken.Issuer);
        Assert.Contains(_jwtSettings.Audience, jwtToken.Audiences);
    }

    [Fact]
    public void GenerateAccessToken_ShouldHaveCorrectExpiration()
    {
        // Arrange
        var service = new JwtTokenService(Options.Create(_jwtSettings));

        // Act
        var token = service.GenerateAccessToken(_user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var expectedExpiry = DateTime.UtcNow.AddSeconds(_jwtSettings.ExpirySeconds);
        Assert.True(jwtToken.ValidTo >= expectedExpiry.AddSeconds(-1.5) &&
                    jwtToken.ValidTo <= expectedExpiry.AddSeconds(1.5));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidEntity()
    {
        // Arrange
        var service = new JwtTokenService(Options.Create(_jwtSettings));

        // Act
        var refreshToken = service.GenerateRefreshToken(_user);

        // Assert
        Assert.Equal(_user.Id, refreshToken.UserId);
        Assert.False(string.IsNullOrWhiteSpace(refreshToken.Token));
        Assert.True(refreshToken.ExpiresAt > DateTime.UtcNow.AddMinutes(59.95));
        Assert.True(refreshToken.ExpiresAt < DateTime.UtcNow.AddMinutes(60.05));
        Assert.Null(refreshToken.RevokedAt);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldReturnPrincipal_ForValidToken()
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            SigningKey = "this_is_a_very_long_and_secure_key",
            Issuer = "test_issuer",
            Audience = "test_audience",
            ExpirySeconds = -10
        };

        var service = new JwtTokenService(Options.Create(jwtSettings));

        var expiredToken = service.GenerateAccessToken(_user);

        // Act
        var principal = service.GetPrincipalFromExpiredToken(expiredToken);

        // Assert
        Assert.Equal(_user.Id.ToString(), principal?.FindFirst(ClaimTypes.Sid)?.Value);
        Assert.Equal(_user.Email, principal?.FindFirst(ClaimTypes.Email)?.Value);
    }

    [Theory]
    [InlineData(Role.Customer)]
    [InlineData(Role.Admin)]
    [InlineData(Role.Owner)]
    public void GenerateAccessToken_ShouldContainCorrectRole(Role role)
    {
        // Arrange
        var service = new JwtTokenService(Options.Create(_jwtSettings));
        _user.UserRole = role;

        // Act
        var token = service.GenerateAccessToken(_user);

        // Assert
        var claims = new JwtSecurityToken(token).Claims;
        Assert.Equal(role.ToString(), claims.First(c => c.Type == ClaimTypes.Role).Value);
    }
}
