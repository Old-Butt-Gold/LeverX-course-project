using System.Security.Claims;
using AutoMapper;
using EER.Application.Abstractions.Security;
using EER.Application.Dto.Security.Login;
using EER.Application.Dto.Security.RefreshToken;
using EER.Application.Dto.Security.RegisterAdmin;
using EER.Application.Dto.Security.RegisterUser;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using FluentValidation;

namespace EER.Application.Services.Security;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMapper _mapper;
    private readonly IValidator<LoginUserDto> _loginUserDtoValidator;
    private readonly IValidator<RegisterUserDto> _registerUserDtoValidator;
    private readonly IValidator<RegisterAdminDto> _registerAdminDtoValidator;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationService(IMapper mapper, IValidator<LoginUserDto> lud, IValidator<RegisterAdminDto> rad,
        IValidator<RegisterUserDto> rud, IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _mapper = mapper;
        _loginUserDtoValidator = lud;
        _registerAdminDtoValidator = rad;
        _registerUserDtoValidator = rud;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken ct = default)
    {
        await _registerUserDtoValidator.ValidateAndThrowAsync(registerUserDto, ct);

        var user = _mapper.Map<User>(registerUserDto);

        user.PasswordHash = _passwordHasher.HashPassword(registerUserDto.Password);

        await _userRepository.AddAsync(user, cancellationToken: ct);
    }

    public async Task RegisterAdminAsync(RegisterAdminDto registerAdminDto, CancellationToken ct = default)
    {
        await _registerAdminDtoValidator.ValidateAndThrowAsync(registerAdminDto, ct);

        var admin = _mapper.Map<User>(registerAdminDto);

        admin.PasswordHash = _passwordHasher.HashPassword(registerAdminDto.Password);

        await _userRepository.AddAsync(admin, cancellationToken: ct);
    }

    public async Task<UserLoggedDto> LoginAsync(LoginUserDto loginUserDto, CancellationToken ct = default)
    {
        await _loginUserDtoValidator.ValidateAndThrowAsync(loginUserDto, cancellationToken: ct);

        var dto = loginUserDto;

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken: ct);

        if (user is null)
        {
            throw new KeyNotFoundException("User with provided email wasn't found");
        }

        var isIdentical = _passwordHasher.VerifyPassword(user.PasswordHash, dto.Password);

        if (!isIdentical)
            return new UserLoggedDto { AccessToken = "", RefreshToken = "", IsSuccess = false };

        var accessToken = _jwtTokenService.GenerateAccessToken(user);

        var entity = _jwtTokenService.GenerateRefreshToken(user);

        await _refreshTokenRepository.AddAsync(entity, cancellationToken: ct);

        return new UserLoggedDto { AccessToken = accessToken, RefreshToken = entity.Token, IsSuccess = isIdentical };
    }

    public async Task<RefreshTokenResultDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken ct = default)
    {
        var dto = refreshTokenDto;
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

        var userId = principal?.FindFirstValue(ClaimTypes.Sid);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
            throw new UnauthorizedAccessException("Invalid token");

        var user = await _userRepository.GetByIdAsync(userIdGuid, cancellationToken: ct);

        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, cancellationToken: ct);

        if (refreshToken == null || refreshToken.UserId != userIdGuid ||
            refreshToken.IsExpired || refreshToken.RevokedAt != null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        refreshToken.Token = newRefreshToken;

        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken: ct);

        return new RefreshTokenResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        await _refreshTokenRepository.RevokeTokenAsync(refreshToken, cancellationToken: ct);
    }

    public async Task LogoutAllAsync(Guid userId, CancellationToken ct = default)
    {
        await _refreshTokenRepository.RevokeAllForUserAsync(userId, cancellationToken: ct);
    }
}
