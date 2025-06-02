using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IEquipmentRepository : IRepository<Equipment, int>
{
    Task<Equipment> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default);
}
