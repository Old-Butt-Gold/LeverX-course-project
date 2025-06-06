using AutoMapper;
using EER.Application.Abstractions.Security;
using EER.Application.Dto.Security.Login;
using EER.Application.Dto.Security.RegisterAdmin;
using EER.Application.Dto.Security.RegisterUser;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace EER.Unit.Tests.Services.Security;

public class AuthenticationServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<LoginUserDto>> _loginValidatorMock = new();
    private readonly Mock<IValidator<RegisterUserDto>> _registerUserValidatorMock = new();
    private readonly Mock<IValidator<RegisterAdminDto>> _registerAdminValidatorMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();

    private readonly Application.Services.Security.AuthenticationService _authService;

    public AuthenticationServiceTests()
    {
        _authService = new(
            _mapperMock.Object,
            _loginValidatorMock.Object,
            _registerAdminValidatorMock.Object,
            _registerUserValidatorMock.Object,
            _userRepoMock.Object,
            _refreshTokenRepoMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object
        );
    }

    private readonly User _testUser = new()
    {
        Id = Guid.NewGuid(),
        Email = "test@example.com",
        PasswordHash = "hashed_password"
    };

    private const string ValidPassword = "ValidPassword123!";
    private const string InvalidPassword = "InvalidPassword";
    private const string AccessToken = "access_token";
    private const string RefreshToken = "refresh_token";

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessResult()
    {
        // Arrange
        SetupValidLogin();
        SetupTokenGeneration();

        var loginDto = new LoginUserDto { Email = _testUser.Email, Password = ValidPassword };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.AccessToken.Should().Be(AccessToken);
        result.RefreshToken.Should().Be(RefreshToken);
        result.UserId.Should().Be(_testUser.Id);

        VerifyTokenSaved();
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        SetupValidLogin();
        _passwordHasherMock.Setup(h => h.VerifyPassword(_testUser.PasswordHash, InvalidPassword))
            .Returns(false);

        var loginDto = new LoginUserDto { Email = _testUser.Email, Password = InvalidPassword };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.AccessToken.Should().BeEmpty();
        result.RefreshToken.Should().BeEmpty();
        result.UserId.Should().Be(Guid.Empty);

        VerifyNoTokenGenerated();
        VerifyNoTokenSaved();
        VerifyNoTokenSaved();
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        const string nonExistingEmail = "notfound@example.com";

        SetupValidationSuccess();
        _userRepoMock.Setup(r => r.GetByEmailAsync(
                nonExistingEmail, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var loginDto = new LoginUserDto { Email = nonExistingEmail, Password = "AnyPassword" };

        // Act & Assert
        var act = async () => await _authService.LoginAsync(loginDto);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task LoginAsync_InvalidInput_ThrowsValidationException()
    {
        // Arrange
        const string invalidEmail = "invalid-email";
        const string shortPassword = "short";

        var validationFailures = new List<ValidationFailure>
        {
            new("Email", "Invalid email format"), new("Password", "Password too short")
        };

        SetupValidationFailure(validationFailures);

        var loginDto = new LoginUserDto { Email = invalidEmail, Password = shortPassword };

        // Act & Assert
        var act = async () => await _authService.LoginAsync(loginDto);
        var exceptionAssertions = await act.Should().ThrowAsync<ValidationException>();

        exceptionAssertions.Which.Errors.Should().HaveCount(2);
        exceptionAssertions.Which.Errors.Select(e => e.ErrorMessage)
            .Should().Contain("Invalid email format", "Password too short");

        VerifyUserNotSearched();
    }

    [Fact]
    public async Task RegisterUserAsync_ValidInput_ShouldAddUser()
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            Email = "user@example.com",
            Password = "Password123!",
            UserRole = Role.Customer
        };

        var user = new User { Id = Guid.NewGuid(), Email = dto.Email };

        _registerUserValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
        _passwordHasherMock.Setup(h => h.HashPassword(dto.Password)).Returns("hashed_password");
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(), null, It.IsAny<CancellationToken>()));

        // Act
        await _authService.RegisterUserAsync(dto);

        // Assert
        _mapperMock.Verify(m => m.Map<User>(dto), Times.Once);
        _passwordHasherMock.Verify(h => h.HashPassword(dto.Password), Times.Once);
        _userRepoMock.Verify(r => r.AddAsync(
                It.Is<User>(u =>
                    u.Email == dto.Email && u.PasswordHash == "hashed_password"),
                null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAdminAsync_ValidInput_ShouldAddAdmin()
    {
        // Arrange
        var dto = new RegisterAdminDto { Email = "admin@example.com", Password = "AdminPassword123!" };

        var admin = new User { Id = Guid.NewGuid(), Email = dto.Email, UserRole = Role.Admin };

        _registerAdminValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(admin);
        _passwordHasherMock.Setup(h => h.HashPassword(dto.Password)).Returns("hashed_password");
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(),
            null, It.IsAny<CancellationToken>()));

        // Act
        await _authService.RegisterAdminAsync(dto);

        // Assert
        _userRepoMock.Verify(r => r.AddAsync(
                It.Is<User>(u => u.Email == dto.Email && u.UserRole == Role.Admin),
                null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_ShouldRevokeToken()
    {
        // Arrange
        const string refreshToken = "token_to_revoke";

        // Act
        await _authService.LogoutAsync(refreshToken);

        // Assert
        _refreshTokenRepoMock.Verify(r => r.RevokeTokenAsync(refreshToken,
            null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogoutAllAsync_ShouldRevokeAllTokensForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _authService.LogoutAllAsync(userId);

        // Assert
        _refreshTokenRepoMock.Verify(r => r.RevokeAllForUserAsync(userId,
            null, It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupValidLogin()
    {
        SetupValidationSuccess();
        _userRepoMock.Setup(r => r.GetByEmailAsync(
                _testUser.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUser);
        _passwordHasherMock.Setup(h => h.VerifyPassword(
                _testUser.PasswordHash, ValidPassword))
            .Returns(true);
    }

    private void SetupTokenGeneration()
    {
        _jwtTokenServiceMock.Setup(s => s.GenerateAccessToken(_testUser))
            .Returns(AccessToken);
        _jwtTokenServiceMock.Setup(s => s.GenerateRefreshToken(_testUser))
            .Returns(new RefreshToken { Token = RefreshToken, UserId = _testUser.Id });
    }

    private void SetupValidationSuccess()
    {
        _loginValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<LoginUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidationFailure(IEnumerable<ValidationFailure> failures)
    {
        var validationResult = new ValidationResult(failures);
        _loginValidatorMock.Setup(v => v.ValidateAsync(
                It.IsAny<LoginUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
    }

    private void VerifyTokenSaved()
    {
        _refreshTokenRepoMock.Verify(r => r.AddAsync(
                It.Is<RefreshToken>(rt => rt.Token == RefreshToken && rt.UserId == _testUser.Id),
                null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private void VerifyNoTokenSaved()
    {
        _refreshTokenRepoMock.Verify(r => r.AddAsync(
                It.IsAny<RefreshToken>(),
                null,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void VerifyNoTokenGenerated()
    {
        _jwtTokenServiceMock.Verify(s =>
            s.GenerateAccessToken(It.IsAny<User>()), Times.Never);
        _jwtTokenServiceMock.Verify(s =>
            s.GenerateRefreshToken(It.IsAny<User>()), Times.Never);
    }

    private void VerifyUserNotSearched()
    {
        _userRepoMock.Verify(r => r.GetByEmailAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

}
