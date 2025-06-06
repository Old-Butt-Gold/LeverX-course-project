using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

internal sealed class UpdateEquipmentItemCommandHandler : IRequestHandler<UpdateEquipmentItemCommand, EquipmentItemUpdatedDto>
{
    private readonly IEquipmentItemRepository _repository;
    private readonly IMapper _mapper;

    public UpdateEquipmentItemCommandHandler(IEquipmentItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EquipmentItemUpdatedDto> Handle(UpdateEquipmentItemCommand command, CancellationToken cancellationToken)
    {
        var dto = command.UpdateEquipmentItemDto;

        var item = await _repository.GetByIdAsync(dto.Id, cancellationToken: cancellationToken);

        if (item is null)
            throw new KeyNotFoundException($"EquipmentItem with ID {dto.Id} not found");

        var mappedItem = _mapper.Map(dto, item);
        item.UpdatedBy = command.Manipulator;

        var updatedItem = await _repository.UpdateAsync(mappedItem, cancellationToken: cancellationToken);

        return _mapper.Map<EquipmentItemUpdatedDto>(updatedItem);
    }
}
