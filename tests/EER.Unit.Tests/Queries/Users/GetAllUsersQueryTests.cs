using AutoMapper;
using EER.Application.Features.Users.Queries.GetAllUsers;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using FluentAssertions;
using Moq;

namespace EER.Unit.Tests.Queries.Users;

public class GetAllUsersQueryTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryTests()
    {
        _handler = new GetAllUsersQueryHandler(_userRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsMappedUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Email = "user1@example.com" },
            new() { Id = Guid.NewGuid(), Email = "user2@example.com" }
        };

        var expectedDtos = new List<UserDto>
        {
            new() { Id = users[0].Id, Email = "user1@example.com" },
            new() { Id = users[1].Id, Email = "user2@example.com" }
        };

        _userRepoMock.Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users))
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task Handle_NoUsers_ReturnsEmptyList()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
