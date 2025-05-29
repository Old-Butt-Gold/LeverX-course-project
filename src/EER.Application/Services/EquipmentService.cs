using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public EquipmentService(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _equipmentRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Equipment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _equipmentRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _equipmentRepository.GetByCategoryAsync(categoryId, cancellationToken);
    }

    public async Task<Equipment> CreateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        return await _equipmentRepository.AddAsync(equipment, cancellationToken);
    }

    public async Task<Equipment> UpdateAsync(int id, Equipment updatedEquipment, CancellationToken cancellationToken = default)
    {
        var existingEquipment = await _equipmentRepository.GetByIdAsync(id, cancellationToken);
        if (existingEquipment is null)
            throw new KeyNotFoundException("Equipment with provided ID is not found");

        existingEquipment.Name = updatedEquipment.Name;
        existingEquipment.CategoryId = updatedEquipment.CategoryId;
        existingEquipment.Description = updatedEquipment.Description;
        existingEquipment.PricePerDay = updatedEquipment.PricePerDay;
        existingEquipment.UpdatedBy = updatedEquipment.UpdatedBy;

        return await _equipmentRepository.UpdateAsync(existingEquipment, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _equipmentRepository.DeleteAsync(id, cancellationToken);
    }
}
