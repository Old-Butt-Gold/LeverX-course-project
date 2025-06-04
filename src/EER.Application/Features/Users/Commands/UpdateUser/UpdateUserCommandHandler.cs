using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserUpdatedDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserUpdatedDto> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var updatedUserDto = command.UpdateUserDto;

        var user = await _userRepository.GetByIdAsync(updatedUserDto.Id, cancellationToken: cancellationToken);

        if (user is null)
            throw new KeyNotFoundException("User with provided ID is not found");

        _mapper.Map(command.UpdateUserDto, user);

        var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken: cancellationToken);
        return _mapper.Map<UserUpdatedDto>(updatedUser);
    }
}
