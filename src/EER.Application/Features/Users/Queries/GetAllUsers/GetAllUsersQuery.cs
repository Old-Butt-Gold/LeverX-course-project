using MediatR;

namespace EER.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
