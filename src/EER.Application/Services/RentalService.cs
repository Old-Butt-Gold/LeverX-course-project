using EER.Application.Abstractions.Services;
using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Application.Services;

internal sealed class RentalService : IRentalService
{
    private readonly Dictionary<int, Rental> _rentals = new();
    private int _idCounter;

    public IEnumerable<Rental> GetAll() => _rentals.Values.ToList();

    public Rental? GetById(int id) => _rentals.GetValueOrDefault(id);

    public Rental Create(Rental rental)
    {
        rental.Id = Interlocked.Increment(ref _idCounter);
        rental.Status = RentalStatus.Pending;
        rental.CreatedAt = DateTime.UtcNow;
        _rentals[rental.Id] = rental;
        return rental;
    }

    public Rental? UpdateStatus(int id, RentalStatus status)
    {
        if (!_rentals.TryGetValue(id, out var rental))
            return null;

        rental.Status = status;
        return rental;
    }

    public bool Delete(int id) => _rentals.Remove(id);
}
