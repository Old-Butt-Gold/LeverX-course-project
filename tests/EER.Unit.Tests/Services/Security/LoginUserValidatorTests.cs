using EER.Application.Dto.Security.Login;
using EER.Application.Validators.Security;
using FluentValidation.TestHelper;

namespace EER.Unit.Tests.Services.Security;

public class LoginUserValidatorTests
{
    private readonly LoginUserValidator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Email_WhenEmpty_ShouldHaveError(string email)
    {
        // Arrange
        var model = new LoginUserDto { Email = email, Password = "ValidPassword1!" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid.email.com")]
    [InlineData("@invalid.com")]
    public void Email_WhenInvalidFormat_ShouldHaveError(string email)
    {
        // Arrange
        var model = new LoginUserDto { Email = email, Password = "ValidPassword1!" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_WhenTooLong_ShouldHaveError()
    {
        // Arrange
        var model = new LoginUserDto
        {
            Email = new string('a', 151) + "@example.com",
            Password = "ValidPassword1!"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Password_WhenEmpty_ShouldHaveError(string password)
    {
        // Arrange
        var model = new LoginUserDto { Email = "test@example.com", Password = password };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("nouppercase1!")]
    [InlineData("NOLOWERCASE1!")]
    [InlineData("NoNumbers!")]
    public void Password_WhenMissingRequiredCharacterType_ShouldHaveError(string password)
    {
        // Arrange
        var model = new LoginUserDto { Email = "test@example.com", Password = password };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Valid_ShouldNotHaveError()
    {
        // Arrange
        var model = new LoginUserDto { Email = "test@example.com", Password = "ValidPassword1!" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}
