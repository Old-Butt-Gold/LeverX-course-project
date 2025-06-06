using EER.Application.Dto.Security.RegisterUser;
using EER.Application.Validators.Security;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Enums;
using FluentValidation.TestHelper;
using Moq;

namespace EER.Unit.Tests.Services.Security;

public class RegisterUserValidatorTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly RegisterUserValidator _validator;

    public RegisterUserValidatorTests()
    {
        _validator = new RegisterUserValidator(_userRepoMock.Object);
    }

    [Theory]
    [InlineData(Role.Admin)]
    public async Task UserRole_WhenAdmin_ShouldHaveError(Role role)
    {
        // Arrange
        var model = new RegisterUserDto
        {
            Email = "user@example.com",
            Password = "ValidPassword1!",
            UserRole = role
        };

        // Act
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserRole);
    }

    [Theory]
    [InlineData(Role.Customer)]
    [InlineData(Role.Owner)]
    public async Task UserRole_WhenNotAdmin_ShouldNotHaveError(Role role)
    {
        // Arrange
        var model = new RegisterUserDto
        {
            Email = "user@example.com",
            Password = "ValidPassword1!",
            UserRole = role
        };

        // Act
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UserRole);
    }
}
