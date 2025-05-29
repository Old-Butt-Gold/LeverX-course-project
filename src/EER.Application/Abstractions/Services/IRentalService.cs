using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Application.Abstractions.Services;

public interface IRentalService
{
    Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Rental> CreateAsync(Rental rental, CancellationToken cancellationToken = default);
    Task<Rental> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
