using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

internal sealed class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, EquipmentUpdatedDto>
{
    private readonly IEquipmentRepository _repository;
    private readonly ILogger<UpdateEquipmentCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateEquipmentCommandHandler(IEquipmentRepository repository, IMapper mapper, ILogger<UpdateEquipmentCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EquipmentUpdatedDto> Handle(UpdateEquipmentCommand command, CancellationToken cancellationToken)
    {
        var updateDto = command.UpdateEquipmentDto;

        var equipment = await _repository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);

        if (equipment is null)
            throw new KeyNotFoundException($"Equipment with ID {updateDto.Id} not found");

        if (equipment.OwnerId != command.Manipulator)
        {
            _logger.LogInformation("User with {userId} tried to update equipment with id {equipmentId} of Owner {ownerId}",
                command.Manipulator, equipment.Id, equipment.OwnerId);

            throw new UnauthorizedAccessException("You have no access to update this equipment");
        }

        var mappedEquipment = _mapper.Map(updateDto, equipment);
        equipment.UpdatedBy = command.Manipulator;

        var updatedEquipment = await _repository.UpdateAsync(mappedEquipment, cancellationToken: cancellationToken);

        return _mapper.Map<EquipmentUpdatedDto>(updatedEquipment);
    }
}
