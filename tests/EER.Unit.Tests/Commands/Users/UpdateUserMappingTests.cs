using AutoMapper;
using EER.Application.Features.Users.Commands.UpdateUser;
using EER.Domain.Entities;
using FluentAssertions;

namespace EER.Unit.Tests.Commands.Users;

public class UpdateUserMappingTests
{
    private readonly IMapper _mapper;

    public UpdateUserMappingTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UpdateUserDto_To_User_Mapping()
    {
        // Arrange
        var dto = new UpdateUserDto
        {
            Id = Guid.NewGuid(),
            Email = "update@example.com",
            FullName = "Updated Name"
        };

        // Act
        var user = _mapper.Map<User>(dto);

        // Assert
        user.Email.Should().Be(dto.Email);
        user.FullName.Should().Be(dto.FullName);
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void User_To_UserUpdatedDto_Mapping()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "mapped@example.com",
            FullName = "Mapped Name",
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<UserUpdatedDto>(user);

        // Assert
        dto.Id.Should().Be(user.Id);
        dto.Email.Should().Be(user.Email);
        dto.FullName.Should().Be(user.FullName);
        dto.UpdatedAt.Should().Be(user.UpdatedAt);
    }
}
