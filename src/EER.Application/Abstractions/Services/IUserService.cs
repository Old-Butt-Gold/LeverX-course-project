using EER.Domain.Entities;

namespace EER.Application.Abstractions.Services;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User? GetById(Guid id);
    User Create(User user);
    User? Update(Guid id, User updatedUser);
    bool Delete(Guid id);
}
