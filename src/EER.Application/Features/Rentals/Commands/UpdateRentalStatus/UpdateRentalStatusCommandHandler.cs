using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

internal sealed class UpdateRentalStatusCommandHandler
    : IRequestHandler<UpdateRentalStatusCommand, RentalUpdatedDto>
{
    private readonly IRentalRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateRentalStatusCommandHandler> _logger;

    public UpdateRentalStatusCommandHandler(IRentalRepository repository, IMapper mapper,
        ILogger<UpdateRentalStatusCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RentalUpdatedDto> Handle(UpdateRentalStatusCommand command, CancellationToken cancellationToken)
    {
        var dto = command.UpdateRentalDto;

        var existingRental = await _repository.GetByIdAsync(dto.Id, cancellationToken: cancellationToken);

        if (existingRental is null)
            throw new KeyNotFoundException($"Rental with provided ID {dto.Id} is not found");

        if (existingRental.OwnerId != command.Manipulator)
        {
            _logger.LogInformation("Owner with {userId} tried to update rental with id {rentalId} of Owner {ownerId}",
                command.Manipulator, existingRental.Id, existingRental.OwnerId);

            throw new UnauthorizedAccessException("You have no access to update this equipment");
        }

        var mappedRental = _mapper.Map(dto, existingRental);
        existingRental.UpdatedBy = command.Manipulator;

        var updatedRental = await _repository.UpdateStatusAsync(mappedRental, command.Manipulator, cancellationToken: cancellationToken);

        return _mapper.Map<RentalUpdatedDto>(updatedRental);
    }
}
