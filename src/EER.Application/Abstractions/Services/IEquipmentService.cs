using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface IEquipmentService
{
    Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<Equipment> CreateAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<Equipment?> UpdateAsync(int id, Equipment updatedEquipment, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
