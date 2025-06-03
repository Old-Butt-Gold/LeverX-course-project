using EER.Application.Abstractions.Security;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Authentication.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserLoggedDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<UserLoggedDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginUserDto;

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken: cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException("User with provided email wasn't found");
        }

        var isIdentical = _passwordHasher.VerifyPassword(user.PasswordHash, dto.Password);

        if (!isIdentical)
            return new UserLoggedDto { AccessToken = "", RefreshToken = "", IsSuccess = false };

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var entity = _jwtTokenService.GenerateRefreshToken(user);

        await _refreshTokenRepository.AddAsync(entity, cancellationToken);

        return new UserLoggedDto { AccessToken = accessToken, RefreshToken = refreshToken, IsSuccess = isIdentical };
    }
}
