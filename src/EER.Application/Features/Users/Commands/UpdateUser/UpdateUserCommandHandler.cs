using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var updatedUser = command.User;

        var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken) ;

        if (user is null)
            throw new KeyNotFoundException("User with provided ID is not found");

        user.Email = updatedUser.Email;
        user.FullName = updatedUser.FullName;
        user.UserRole = updatedUser.UserRole;
        user.UpdatedAt = DateTime.UtcNow;

        return await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
