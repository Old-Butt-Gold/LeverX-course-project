using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Authentication.Commands.Logout;

internal sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokenRepository.RevokeTokenAsync(request.RefreshToken, cancellationToken: cancellationToken);
    }
}
