using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

internal sealed class CreateRentalCommandHandler
    : IRequestHandler<CreateRentalCommand, RentalCreatedDto>
{
    private readonly IRentalRepository _repository;
    private readonly IMapper _mapper;

    public CreateRentalCommandHandler(IRentalRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RentalCreatedDto> Handle(CreateRentalCommand command, CancellationToken cancellationToken)
    {
        // TODO work with equipmentItems ids and array, set their ItemState as Active and etc.
        // TODO and check that their current state isn't InUse, UnderMaintenance, Retired
        // Total Price will be send as
        var rental = _mapper.Map<Rental>(command.CreateRentalDto);
        rental.CreatedBy = command.Manipulator;
        rental.UpdatedBy = command.Manipulator;

        var createdRental = await _repository.AddAsync(rental, cancellationToken: cancellationToken);

        return _mapper.Map<RentalCreatedDto>(createdRental);
    }
}
