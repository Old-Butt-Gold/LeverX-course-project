using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EER.Application.Settings;
using EER.Domain.Entities;
using EER.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace EER.Unit.Tests.Services.Security;

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
        var service = new Application.Services.Security.JwtTokenService(Options.Create(_jwtSettings));

        // Act
        var token = service.GenerateAccessToken(_user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        jwtToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value.Should().Be(_user.Id.ToString());
        jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value.Should().Be(_user.Email);
        jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be(_user.UserRole.ToString());
        jwtToken.Issuer.Should().Be(_jwtSettings.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtSettings.Audience);
    }

    [Fact]
    public void GenerateAccessToken_ShouldHaveCorrectExpiration()
    {
        // Arrange
        var service = new Application.Services.Security.JwtTokenService(Options.Create(_jwtSettings));

        // Act
        var token = service.GenerateAccessToken(_user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var expectedExpiry = DateTime.UtcNow.AddSeconds(_jwtSettings.ExpirySeconds);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(1.5));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidEntity()
    {
        // Arrange
        var service = new Application.Services.Security.JwtTokenService(Options.Create(_jwtSettings));

        // Act
        var refreshToken = service.GenerateRefreshToken(_user);

        // Assert
        refreshToken.UserId.Should().Be(_user.Id);
        refreshToken.Token.Should().NotBeNullOrWhiteSpace();

        var expectedExpiry = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshExpirySeconds);
        refreshToken.ExpiresAt.Should().BeCloseTo(expectedExpiry, TimeSpan.FromMilliseconds(50));
        refreshToken.RevokedAt.Should().BeNull();
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

        var service = new Application.Services.Security.JwtTokenService(Options.Create(jwtSettings));

        var expiredToken = service.GenerateAccessToken(_user);

        // Act
        var principal = service.GetPrincipalFromExpiredToken(expiredToken);

        // Assert
        principal?.FindFirst(ClaimTypes.Sid)?.Value.Should().Be(_user.Id.ToString());
        principal?.FindFirst(ClaimTypes.Email)?.Value.Should().Be(_user.Email);
    }

    [Theory]
    [InlineData(Role.Customer)]
    [InlineData(Role.Admin)]
    [InlineData(Role.Owner)]
    public void GenerateAccessToken_ShouldContainCorrectRole(Role role)
    {
        // Arrange
        var service = new Application.Services.Security.JwtTokenService(Options.Create(_jwtSettings));
        _user.UserRole = role;

        // Act
        var token = service.GenerateAccessToken(_user);

        // Assert
        var claims = new JwtSecurityToken(token).Claims;
        claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be(role.ToString());
    }
}
