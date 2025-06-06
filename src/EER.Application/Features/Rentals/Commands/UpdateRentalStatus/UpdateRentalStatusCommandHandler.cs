using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

internal sealed class UpdateRentalStatusCommandHandler
    : IRequestHandler<UpdateRentalStatusCommand, RentalUpdatedDto>
{
    private readonly IRentalRepository _repository;
    private readonly IMapper _mapper;

    public UpdateRentalStatusCommandHandler(IRentalRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RentalUpdatedDto> Handle(UpdateRentalStatusCommand command, CancellationToken cancellationToken)
    {
        var dto = command.UpdateRentalDto;

        var existingRental = await _repository.GetByIdAsync(dto.Id, cancellationToken: cancellationToken);

        if (existingRental is null)
            throw new KeyNotFoundException($"Rental with provided ID {dto.Id} is not found");

        // TODO Add if existingRental.Status is [Canceled, Completed] then throw exception that you can't change
        // status of this rental anymore

        var mappedRental = _mapper.Map(dto, existingRental);
        existingRental.UpdatedBy = command.Manipulator;

        var updatedRental = await _repository.UpdateStatusAsync(mappedRental, cancellationToken: cancellationToken);

        return _mapper.Map<RentalUpdatedDto>(updatedRental);
    }
}
