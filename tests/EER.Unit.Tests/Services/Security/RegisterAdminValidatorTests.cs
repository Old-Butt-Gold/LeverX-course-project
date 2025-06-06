using EER.Application.Dto.Security.RegisterAdmin;
using EER.Application.Validators.Security;
using EER.Domain.DatabaseAbstractions;
using FluentValidation.TestHelper;
using Moq;

namespace EER.Unit.Tests.Services.Security;

public class RegisterAdminValidatorTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly RegisterAdminValidator _validator;

    public RegisterAdminValidatorTests()
    {
        _validator = new RegisterAdminValidator(_userRepoMock.Object);
    }

    [Fact]
    public async Task Email_WhenExists_ShouldHaveError()
    {
        // Arrange
        const string existingEmail = "admin@example.com";
        var model = new RegisterAdminDto { Email = existingEmail, Password = "ValidPassword1!" };

        _userRepoMock.Setup(r => r.IsEmailExistsAsync(existingEmail,
                null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email already exists");
    }

    [Fact]
    public async Task Email_WhenNotExists_ShouldNotHaveError()
    {
        // Arrange
        const string newEmail = "new@example.com";
        var model = new RegisterAdminDto { Email = newEmail, Password = "ValidPassword1!" };

        _userRepoMock.Setup(r => r.IsEmailExistsAsync(newEmail,
                null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}
