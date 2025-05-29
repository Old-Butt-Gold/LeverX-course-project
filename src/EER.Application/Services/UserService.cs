using EER.Application.Abstractions.Security;
using EER.Application.Abstractions.Services;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;

namespace EER.Application.Services;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetAllAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
        return await _userRepository.AddAsync(user, cancellationToken);
    }

    public async Task<User?> UpdateAsync(Guid id, User updatedUser, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (existingUser is null) return null;

        existingUser.Email = updatedUser.Email;
        existingUser.FullName = updatedUser.FullName;
        existingUser.PasswordHash = _passwordHasher.HashPassword(updatedUser.PasswordHash);
        existingUser.UserRole = updatedUser.UserRole;

        return await _userRepository.UpdateAsync(existingUser, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.DeleteAsync(id, cancellationToken);
    }
}
