using MediatR;

namespace EER.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(CreateUserDto CreateUserDto) : IRequest<UserCreatedDto>;
