using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IOfficeRepository : IRepository<Office, int>
{
    Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Office> AddAsync(Office office, CancellationToken cancellationToken = default);
    Task<Office> UpdateAsync(Office office, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
