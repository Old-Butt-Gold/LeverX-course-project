using EER.Application.Abstractions.Security;
using EER.Application.Abstractions.Services;
using EER.Domain.Entities;

namespace EER.Application.Services;

public class UserService : IUserService
{
    private readonly Dictionary<Guid, User> _users = [];
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public IEnumerable<User> GetAll() => _users.Values.ToList();

    public User? GetById(Guid id) => _users.GetValueOrDefault(id);

    public User Create(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
        _users[user.Id] = user;
        return user;
    }

    public User? Update(Guid id, User updatedUser)
    {
        if (!_users.TryGetValue(id, out var user))
            return null;

        user.Email = updatedUser.Email;
        user.FullName = updatedUser.FullName;
        user.PasswordHash = _passwordHasher.HashPassword(updatedUser.PasswordHash);
        return user;
    }

    public bool Delete(Guid id) => _users.Remove(id);
}
