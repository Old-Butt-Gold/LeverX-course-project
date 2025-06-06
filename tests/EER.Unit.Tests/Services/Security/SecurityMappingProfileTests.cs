using AutoMapper;
using EER.Application.Dto.Security.RegisterAdmin;
using EER.Application.Dto.Security.RegisterUser;
using EER.Application.MappingProfiles.Security;
using EER.Domain.Entities;
using EER.Domain.Enums;
using FluentAssertions;

namespace EER.Unit.Tests.Services.Security;

public class SecurityMappingProfileTests
{
    private readonly IMapper _mapper;

    public SecurityMappingProfileTests()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void RegisterUserDto_To_User_Mapping()
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            Email = "user@example.com",
            Password = "Password123",
            UserRole = Role.Customer
        };

        // Act
        var result = _mapper.Map<User>(dto);

        // Assert
        result.Email.Should().Be(dto.Email);
        result.UserRole.Should().Be(dto.UserRole);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.PasswordHash.Should().BeNull();
        result.FullName.Should().BeNull();
    }

    [Fact]
    public void RegisterAdminDto_To_User_Mapping()
    {
        // Arrange
        var dto = new RegisterAdminDto
        {
            Email = "admin@example.com",
            Password = "AdminPassword123"
        };

        // Act
        var result = _mapper.Map<User>(dto);

        // Assert
        result.Email.Should().Be(dto.Email);
        result.UserRole.Should().Be(Role.Admin);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.PasswordHash.Should().BeNull();
        result.FullName.Should().BeNull();
    }
}
