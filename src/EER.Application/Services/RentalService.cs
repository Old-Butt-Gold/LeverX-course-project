using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Application.Services;

internal sealed class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;

    public RentalService(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _rentalRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _rentalRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Rental> CreateAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        rental.Status = RentalStatus.Pending;
        return await _rentalRepository.AddAsync(rental, cancellationToken);
    }

    public async Task<Rental> UpdateStatusAsync(int id, RentalStatus status, Guid updatedBy, CancellationToken cancellationToken = default)
    {
        var existingRental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        if (existingRental is null)
            throw new KeyNotFoundException("Rental with provided ID is not found");

        return await _rentalRepository.UpdateStatusAsync(id, status, updatedBy, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _rentalRepository.DeleteAsync(id, cancellationToken);
    }
}
