using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.DeleteRental;

internal sealed class DeleteRentalCommandHandler : IRequestHandler<DeleteRentalCommand, bool>
{
    private readonly IRentalRepository _repository;

    public DeleteRentalCommandHandler(IRentalRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteRentalCommand command, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(command.Id, cancellationToken);
    }
}
