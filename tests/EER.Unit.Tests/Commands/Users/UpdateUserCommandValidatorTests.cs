using EER.Application.Features.Users.Commands.UpdateUser;
using EER.Domain.DatabaseAbstractions;
using FluentValidation.TestHelper;
using Moq;

namespace EER.Unit.Tests.Commands.Users;

public class UpdateUserCommandValidatorTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly UpdateUserCommandValidator _validator;

    public UpdateUserCommandValidatorTests()
    {
        _validator = new UpdateUserCommandValidator(_userRepoMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new UpdateUserCommand(new UpdateUserDto
        {
            Id = Guid.NewGuid(),
            Email = "valid@example.com",
            FullName = "Valid Name"
        });

        _userRepoMock.Setup(r => r.IsEmailExistsAsync(
                command.UpdateUserDto.Email, command.UpdateUserDto.Id,
                null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmailExists_ShouldFail()
    {
        // Arrange
        var command = new UpdateUserCommand(new UpdateUserDto
        {
            Id = Guid.NewGuid(),
            Email = "exists@example.com",
            FullName = "Valid Name"
        });

        _userRepoMock.Setup(r => r.IsEmailExistsAsync(
                command.UpdateUserDto.Email, command.UpdateUserDto.Id,
                null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateUserDto.Email)
            .WithErrorMessage("Email already exists");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_InvalidEmail_ShouldFail(string email)
    {
        // Arrange
        var command = new UpdateUserCommand(new UpdateUserDto
        {
            Id = Guid.NewGuid(),
            Email = email
        });

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateUserDto.Email);
    }

    [Fact]
    public async Task Validate_LongFullName_ShouldFail()
    {
        // Arrange
        var command = new UpdateUserCommand(new UpdateUserDto
        {
            Id = Guid.NewGuid(),
            Email = "valid@example.com",
            FullName = new string('a', 151)
        });

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateUserDto.FullName);
    }

    [Fact]
    public async Task UpdateUserCommand_WithExistingEmail_ShouldFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string existingEmail = "exists@example.com";

        var command = new UpdateUserCommand(new UpdateUserDto
        {
            Id = userId,
            Email = existingEmail
        });

        _userRepoMock.Setup(r => r.IsEmailExistsAsync(
                existingEmail, userId,
                null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateUserDto.Email)
            .WithErrorMessage("Email already exists");
    }
}
