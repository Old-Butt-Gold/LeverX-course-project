using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IEquipmentRepository : IRepository<Equipment, int>
{
    Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<Equipment> AddAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<Equipment?> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
