using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

internal sealed class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, EquipmentUpdatedDto>
{
    private readonly IEquipmentRepository _repository;
    private readonly IMapper _mapper;

    public UpdateEquipmentCommandHandler(IEquipmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EquipmentUpdatedDto> Handle(UpdateEquipmentCommand command, CancellationToken cancellationToken)
    {
        var updateDto = command.UpdateEquipmentDto;

        var equipment = await _repository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);

        if (equipment is null)
            throw new KeyNotFoundException($"Equipment with ID {updateDto.Id} not found");

        _mapper.Map(updateDto, equipment);
        equipment.UpdatedBy = command.Manipulator;

        var updatedEquipment = await _repository.UpdateAsync(equipment, cancellationToken: cancellationToken);

        return _mapper.Map<EquipmentUpdatedDto>(updatedEquipment);
    }
}
