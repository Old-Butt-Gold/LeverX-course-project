using EER.Application.Abstractions.Security;
using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUnitOfWork uow, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.UserRepository.GetAllAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _uow.UserRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
        return await _uow.UserRepository.AddAsync(user, cancellationToken);
    }

    public async Task<User?> UpdateAsync(Guid id, User updatedUser, CancellationToken cancellationToken = default)
    {
        var existingUser = await _uow.UserRepository.GetByIdAsync(id, cancellationToken);
        if (existingUser == null) return null;

        existingUser.Email = updatedUser.Email;
        existingUser.FullName = updatedUser.FullName;
        existingUser.PasswordHash = _passwordHasher.HashPassword(updatedUser.PasswordHash);
        existingUser.UserRole = updatedUser.UserRole;

        return await _uow.UserRepository.UpdateAsync(existingUser, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _uow.UserRepository.DeleteAsync(id, cancellationToken);
    }
}
