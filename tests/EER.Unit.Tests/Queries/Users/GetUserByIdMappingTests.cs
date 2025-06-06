using AutoMapper;
using EER.Application.Features.Users.Queries.GetUserById;
using EER.Domain.Entities;
using EER.Domain.Enums;
using FluentAssertions;

namespace EER.Unit.Tests.Queries.Users;

public class GetUserByIdMappingTests
{
    private readonly IMapper _mapper;

    public GetUserByIdMappingTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void User_To_UserDetailsDto_Mapping()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "John Doe",
            UserRole = Role.Customer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<UserDetailsDto>(user);

        // Assert
        dto.Id.Should().Be(user.Id);
        dto.Email.Should().Be(user.Email);
        dto.FullName.Should().Be(user.FullName);
        dto.UserRole.Should().Be(user.UserRole);
        dto.CreatedAt.Should().Be(user.CreatedAt);
        dto.UpdatedAt.Should().Be(user.UpdatedAt);
    }
}
