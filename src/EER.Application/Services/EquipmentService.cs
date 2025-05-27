using EER.Application.Abstractions.Services;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class EquipmentService : IEquipmentService
{
    private readonly Dictionary<int, Equipment> _equipment = [];
    private int _idCounter;

    public IEnumerable<Equipment> GetAll() => _equipment.Values.ToList();

    public Equipment? GetById(int id) => _equipment.GetValueOrDefault(id);

    public IEnumerable<Equipment> GetByCategory(int categoryId) =>
        _equipment.Values.Where(e => e.CategoryId == categoryId).ToList();

    public Equipment Create(Equipment equipment)
    {
        equipment.Id = Interlocked.Increment(ref _idCounter);
        equipment.CreatedAt = DateTime.UtcNow;
        _equipment[equipment.Id] = equipment;
        return equipment;
    }

    public Equipment? Update(int id, Equipment updatedEquipment)
    {
        if (!_equipment.TryGetValue(id, out var equipment))
            return null;

        equipment.Name = updatedEquipment.Name;
        equipment.CategoryId = updatedEquipment.CategoryId;
        equipment.Description = updatedEquipment.Description;
        equipment.PricePerDay = updatedEquipment.PricePerDay;
        equipment.AverageRating = updatedEquipment.AverageRating;
        equipment.TotalReviews = updatedEquipment.TotalReviews;
        return equipment;
    }

    public bool Delete(int id) => _equipment.Remove(id);
}
