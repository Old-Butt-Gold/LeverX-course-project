using EER.Application.Abstractions.Services;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class OfficeService : IOfficeService
{
    private readonly Dictionary<int, Office> _offices = [];
    private int _idCounter;

    public IEnumerable<Office> GetAll() => _offices.Values.ToList();

    public Office? GetById(int id) => _offices.GetValueOrDefault(id);

    public Office Create(Office office)
    {
        office.Id = Interlocked.Increment(ref _idCounter);
        _offices[office.Id] = office;
        return office;
    }

    public Office? Update(int id, Office updatedOffice)
    {
        if (!_offices.TryGetValue(id, out var office))
            return null;

        office.OwnerId = updatedOffice.OwnerId;
        office.Address = updatedOffice.Address;
        office.City = updatedOffice.City;
        office.Country = updatedOffice.Country;
        office.IsActive = updatedOffice.IsActive;
        return office;
    }

    public bool Delete(int id) => _offices.Remove(id);
}
