using AutoMapper;
using EER.Application.Features.Users.Queries.GetUserById;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;
using Moq;

namespace EER.Unit.Tests.Queries;

public class GetUserByIdQueryTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryTests()
    {
        _handler = new GetUserByIdQueryHandler(_userRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsUserDetailsDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "passwordHash123!",
            UserRole = Role.Customer
        };

        var expectedDto = new UserDetailsDto
        {
            Id = userId,
            Email = "test@example.com",
            UserRole = Role.Customer
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(
                userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDetailsDto>(user))
            .Returns(expectedDto);

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal(Role.Customer, result.UserRole);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(r => r.GetByIdAsync(
                userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }


}
