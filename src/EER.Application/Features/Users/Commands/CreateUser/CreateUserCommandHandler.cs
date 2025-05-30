using EER.Application.Abstractions.Security;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = command.Email,
            FullName = command.FullName,
            PasswordHash = _passwordHasher.HashPassword(command.PasswordHash),
            UserRole = command.UserRole,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        return await _userRepository.AddAsync(user, cancellationToken);
    }
}
