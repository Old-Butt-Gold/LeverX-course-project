using EER.Domain.Entities;
using EER.Domain.Enums;
using MediatR;

namespace EER.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string? FullName,
    string PasswordHash,
    Role UserRole) : IRequest<User>;
