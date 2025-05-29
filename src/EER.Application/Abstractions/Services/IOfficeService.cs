using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface IOfficeService
{
    Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Office> CreateAsync(Office office, CancellationToken cancellationToken = default);
    Task<Office> UpdateAsync(int id, Office updatedOffice, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
