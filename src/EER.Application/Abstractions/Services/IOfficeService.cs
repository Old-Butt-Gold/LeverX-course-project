using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface IOfficeService
{
    IEnumerable<Office> GetAll();
    Office? GetById(int id);
    Office Create(Office office);
    Office? Update(int id, Office updatedOffice);
    bool Delete(int id);
}
