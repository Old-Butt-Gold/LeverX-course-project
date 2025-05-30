using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<User?>;
