using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Application.Services;

internal sealed class RentalService : IRentalService
{
    private readonly IUnitOfWork _uow;

    public RentalService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.RentalRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.RentalRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Rental> CreateAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        rental.Status = RentalStatus.Pending;
        return await _uow.RentalRepository.AddAsync(rental, cancellationToken);
    }

    public async Task<Rental?> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default)
    {
        return await _uow.RentalRepository.UpdateStatusAsync(id, status, updatedBy, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.RentalRepository.DeleteAsync(id, cancellationToken);
    }
}
