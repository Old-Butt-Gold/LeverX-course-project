using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using MediatR;

namespace EER.Application.Features.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserUpdatedDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private ITransactionManager _transactionManager;

    public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper, ITransactionManager transactionManager)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _transactionManager = transactionManager;
    }

    public async Task<UserUpdatedDto> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var updatedUserDto = command.UpdateUserDto;


        var user = await _userRepository.GetByIdAsync(updatedUserDto.Id, cancellationToken: cancellationToken);

        await using var i = await _transactionManager.BeginTransactionAsync(ct: cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User with provided ID is not found");

        _mapper.Map(command.UpdateUserDto, user);

        // TODO check if updated Email is unique still

        var updatedUser = await _userRepository.UpdateAsync(user, i, cancellationToken: cancellationToken);
        await i.RollbackAsync(cancellationToken);
        return _mapper.Map<UserUpdatedDto>(updatedUser);
    }
}
