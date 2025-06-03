using MediatR;

namespace EER.Application.Features.Authentication.Commands.LoginUser;

public record LoginUserCommand(LoginUserDto LoginUserDto)
    : IRequest<UserLoggedDto>;
