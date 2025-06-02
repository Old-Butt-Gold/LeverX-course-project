using EER.Domain.Entities;

namespace EER.Domain.DatabaseAbstractions;

public interface IOfficeRepository : IRepository<Office, int>
{
    Task<Office> UpdateAsync(Office office, CancellationToken cancellationToken = default);
}
