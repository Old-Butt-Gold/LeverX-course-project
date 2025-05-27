using EER.Application.Abstractions.Services;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class EquipmentItemService : IEquipmentItemService
{
    private readonly Dictionary<long, EquipmentItem> _items = [];
    private long _idCounter;

    public IEnumerable<EquipmentItem> GetAll() => _items.Values.ToList();

    public EquipmentItem? GetById(long id) => _items.GetValueOrDefault(id);

    public EquipmentItem Create(EquipmentItem item)
    {
        item.Id = Interlocked.Increment(ref _idCounter);
        _items[item.Id] = item;
        return item;
    }

    public EquipmentItem? Update(long id, EquipmentItem updatedItem)
    {
        if (!_items.TryGetValue(id, out var item))
            return null;

        item.EquipmentId = updatedItem.EquipmentId;
        item.OfficeId = updatedItem.OfficeId;
        item.SerialNumber = updatedItem.SerialNumber;
        item.ItemStatus = updatedItem.ItemStatus;
        item.MaintenanceDate = updatedItem.MaintenanceDate;
        item.PurchaseDate = updatedItem.PurchaseDate;
        return item;
    }

    public bool Delete(long id) => _items.Remove(id);
}
