// EER.Application.Services/OfficeService.cs
using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class OfficeService : IOfficeService
{
    private readonly IOfficeRepository _officeRepository;

    public OfficeService(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository;
    }

    public async Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _officeRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Office?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _officeRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Office> CreateAsync(Office office, CancellationToken cancellationToken = default)
    {
        return await _officeRepository.AddAsync(office, cancellationToken);
    }

    public async Task<Office?> UpdateAsync(int id, Office updatedOffice, CancellationToken cancellationToken = default)
    {
        var existingOffice = await _officeRepository.GetByIdAsync(id, cancellationToken);
        if (existingOffice is null) return null;

        existingOffice.OwnerId = updatedOffice.OwnerId;
        existingOffice.Address = updatedOffice.Address;
        existingOffice.City = updatedOffice.City;
        existingOffice.Country = updatedOffice.Country;
        existingOffice.IsActive = updatedOffice.IsActive;
        existingOffice.UpdatedBy = updatedOffice.UpdatedBy;

        return await _officeRepository.UpdateAsync(existingOffice, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _officeRepository.DeleteAsync(id, cancellationToken);
    }
}
