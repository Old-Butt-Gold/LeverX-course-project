using AutoMapper;
using EER.Application.Abstractions.Security;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserCreatedDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<UserCreatedDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // TODO Email Unique with BadRequest later

        var user = _mapper.Map<User>(command.CreateUserDto);

        user.PasswordHash = _passwordHasher.HashPassword(command.CreateUserDto.Password);

        var createdUser = await _userRepository.AddAsync(user, cancellationToken);

        return _mapper.Map<UserCreatedDto>(createdUser);
    }
}
