using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;

internal sealed class CreateEquipmentItemCommandHandler : IRequestHandler<CreateEquipmentItemCommand, EquipmentItemCreatedDto>
{
    private readonly IEquipmentItemRepository _repository;
    private readonly IMapper _mapper;

    public CreateEquipmentItemCommandHandler(IEquipmentItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EquipmentItemCreatedDto> Handle(CreateEquipmentItemCommand command, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<EquipmentItem>(command.CreateEquipmentItemDto);
        item.CreatedBy = command.Manipulator;
        item.UpdatedBy = command.Manipulator;

        var createdItem = await _repository.AddAsync(item, cancellationToken: cancellationToken);

        return _mapper.Map<EquipmentItemCreatedDto>(createdItem);
    }
}
