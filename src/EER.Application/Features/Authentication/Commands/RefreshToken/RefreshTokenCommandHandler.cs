using System.Security.Claims;
using EER.Application.Abstractions.Security;
using EER.Domain.DatabaseAbstractions;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace EER.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<RefreshTokenResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RefreshTokenDto;
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

        var userId = principal?.FindFirstValue(ClaimTypes.Sid);

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
            throw new SecurityTokenException("Invalid token");

        var user = await _userRepository.GetByIdAsync(userIdGuid, cancellationToken: cancellationToken);

        if (user is null)
            throw new SecurityTokenException("User not found");

        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, cancellationToken);

        if (refreshToken == null || refreshToken.UserId != userIdGuid ||
            refreshToken.IsExpired || refreshToken.RevokedAt != null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        refreshToken.Token = newRefreshToken;

        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken: cancellationToken);

        return new RefreshTokenResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
