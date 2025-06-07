using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IEquipmentRepository : IRepository<Equipment, int>
{
    Task<Equipment> UpdateAsync(Equipment equipment, ITransaction? transaction = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetUnmoderatedAsync(ITransaction? transaction = null, CancellationToken cancellationToken = default);
}
