using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _uow;

    public EquipmentService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentRepository.GetByCategoryAsync(categoryId, cancellationToken);
    }

    public async Task<Equipment> CreateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentRepository.AddAsync(equipment, cancellationToken);
    }

    public async Task<Equipment?> UpdateAsync(int id, Equipment updatedEquipment, CancellationToken cancellationToken = default)
    {
        var existingEquipment = await _uow.EquipmentRepository.GetByIdAsync(id, cancellationToken);
        if (existingEquipment is null) return null;

        existingEquipment.Name = updatedEquipment.Name;
        existingEquipment.CategoryId = updatedEquipment.CategoryId;
        existingEquipment.Description = updatedEquipment.Description;
        existingEquipment.PricePerDay = updatedEquipment.PricePerDay;
        existingEquipment.UpdatedBy = updatedEquipment.UpdatedBy;

        return await _uow.EquipmentRepository.UpdateAsync(existingEquipment, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.EquipmentRepository.DeleteAsync(id, cancellationToken);
    }
}
