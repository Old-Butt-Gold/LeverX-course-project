using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

internal sealed class UpdateRentalStatusCommandHandler : IRequestHandler<UpdateRentalStatusCommand, Rental>
{
    private readonly IRentalRepository _repository;

    public UpdateRentalStatusCommandHandler(IRentalRepository repository)
    {
        _repository = repository;
    }

    public async Task<Rental> Handle(UpdateRentalStatusCommand command, CancellationToken cancellationToken)
    {
        var existingRental = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (existingRental is null)
            throw new KeyNotFoundException("Rental with provided ID is not found");

        // TODO UpdatedBy

        return await _repository.UpdateStatusAsync(
            command.Id, command.Status,
            Guid.NewGuid(), cancellationToken);
    }
}
