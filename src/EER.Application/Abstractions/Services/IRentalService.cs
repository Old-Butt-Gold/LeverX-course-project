using EER.Domain.Entities;
using EER.Domain.Enums;

namespace EER.Application.Abstractions.Services;

public interface IRentalService
{
    IEnumerable<Rental> GetAll();
    Rental? GetById(int id);
    Rental Create(Rental rental);
    Rental? UpdateStatus(int id, RentalStatus status);
    bool Delete(int id);
}
