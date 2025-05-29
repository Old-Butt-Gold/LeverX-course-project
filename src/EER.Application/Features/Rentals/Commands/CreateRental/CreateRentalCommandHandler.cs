using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

internal sealed class CreateRentalCommandHandler : IRequestHandler<CreateRentalCommand, Rental>
{
    private readonly IRentalRepository _repository;

    public CreateRentalCommandHandler(IRentalRepository repository)
    {
        _repository = repository;
    }

    public async Task<Rental> Handle(CreateRentalCommand command, CancellationToken cancellationToken)
    {
        // TODO UpdatedBy
        var rental = new Rental
        {
            OwnerId = command.OwnerId,
            CustomerId = command.CustomerId,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            TotalPrice = command.TotalPrice,
            Status = RentalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = Guid.NewGuid(),
        };

        return await _repository.AddAsync(rental, cancellationToken);
    }
}
