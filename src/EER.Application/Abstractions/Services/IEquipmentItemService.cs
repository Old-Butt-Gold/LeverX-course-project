using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface IEquipmentItemService
{
    IEnumerable<EquipmentItem> GetAll();
    EquipmentItem? GetById(long id);
    EquipmentItem Create(EquipmentItem item);
    EquipmentItem? Update(long id, EquipmentItem updatedItem);
    bool Delete(long id);
}
