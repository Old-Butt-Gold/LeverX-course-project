using EER.Application.Features.Users.Commands.DeleteUser;
using EER.Domain.DatabaseAbstractions;
using FluentAssertions;
using Moq;

namespace EER.Unit.Tests.Commands.Users;

public class DeleteUserCommandTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandTests()
    {
        _handler = new DeleteUserCommandHandler(_userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_UserDeleted_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.DeleteAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UserNotDeleted_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(r => r.DeleteAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
