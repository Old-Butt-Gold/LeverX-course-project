using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface IEquipmentService
{
    IEnumerable<Equipment> GetAll();
    Equipment? GetById(int id);
    IEnumerable<Equipment> GetByCategory(int categoryId);
    Equipment Create(Equipment equipment);
    Equipment? Update(int id, Equipment updatedEquipment);
    bool Delete(int id);
}
