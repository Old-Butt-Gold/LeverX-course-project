using AutoMapper;
using EER.Application.Features.Users.Commands.UpdateUser;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using FluentAssertions;
using Moq;

namespace EER.Unit.Tests.Commands.Users;

public class UpdateUserCommandTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandTests()
    {
        _handler = new UpdateUserCommandHandler(_userRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateDto = new UpdateUserDto
        {
            Id = userId,
            Email = "updated@example.com",
            FullName = "Updated Name"
        };

        var user = new User
        {
            Id = userId,
            Email = "old@example.com"
        };

        var updatedUser = new User
        {
            Id = userId,
            Email = "updated@example.com",
            FullName = "Updated Name"
        };

        var expectedDto = new UserUpdatedDto
        {
            Id = userId,
            Email = "updated@example.com",
            FullName = "Updated Name"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>(),
                null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedUser);

        _mapperMock.Setup(m => m.Map(updateDto, user))
            .Returns(updatedUser);

        _mapperMock.Setup(m => m.Map<UserUpdatedDto>(updatedUser))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(new UpdateUserCommand(updateDto), It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _userRepoMock.Verify(r => r.UpdateAsync(updatedUser,
            null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateDto = new UpdateUserDto
        {
            Id = userId,
            Email = "example@gmail.com"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new UpdateUserCommand(updateDto), It.IsAny<CancellationToken>()));
    }
}
