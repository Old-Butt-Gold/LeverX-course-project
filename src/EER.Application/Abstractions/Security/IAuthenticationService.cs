using EER.Application.Dto.Security.Login;
using EER.Application.Dto.Security.RefreshToken;
using EER.Application.Dto.Security.RegisterAdmin;
using EER.Application.Dto.Security.RegisterUser;

namespace EER.Application.Abstractions.Security;

public interface IAuthenticationService
{
    public Task RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken ct = default);
    public Task RegisterAdminAsync(RegisterAdminDto registerAdminDto, CancellationToken ct = default);
    public Task<UserLoggedDto> LoginAsync(LoginUserDto loginUserDto, CancellationToken ct = default);
    public Task<RefreshTokenResultDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken ct = default);
    public Task LogoutAsync(string refreshToken, CancellationToken ct = default);
    public Task LogoutAllAsync(Guid userId, CancellationToken ct = default);
}
