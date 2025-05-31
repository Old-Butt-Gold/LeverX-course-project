using MediatR;

namespace EER.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(UpdateUserDto UpdateUserDto) : IRequest<UserUpdatedDto>;
