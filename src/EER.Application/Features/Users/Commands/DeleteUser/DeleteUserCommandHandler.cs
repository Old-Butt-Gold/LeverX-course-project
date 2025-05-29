using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        return await _userRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
