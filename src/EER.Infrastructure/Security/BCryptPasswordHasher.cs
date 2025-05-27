using EER.Application.Abstractions.Security;

namespace EER.Infrastructure.Security;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // It fits in 60 characters, there are 64 characters size in the database.
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
