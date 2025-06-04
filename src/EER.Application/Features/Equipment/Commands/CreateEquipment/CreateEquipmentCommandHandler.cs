using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

internal sealed class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, EquipmentCreatedDto>
{
    private readonly IEquipmentRepository _repository;
    private readonly IMapper _mapper;

    public CreateEquipmentCommandHandler(IEquipmentRepository repository, IMapper mapper)
    {
        _repository = repository;

        _mapper = mapper;
    }

    public async Task<EquipmentCreatedDto> Handle(CreateEquipmentCommand command, CancellationToken cancellationToken)
    {
        var equipment = _mapper.Map<Domain.Entities.Equipment>(command.CreateEquipmentDto);
        equipment.CreatedBy = command.Manipulator;
        equipment.UpdatedBy = command.Manipulator;

        var createdEquipment = await _repository.AddAsync(equipment, cancellationToken: cancellationToken);

        return _mapper.Map<EquipmentCreatedDto>(createdEquipment);
    }
}
