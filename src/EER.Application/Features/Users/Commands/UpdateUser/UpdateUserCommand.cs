using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, User User) : IRequest<User>;
