using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Authentication.Commands.LogoutAll;

public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutAllCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokenRepository.RevokeAllForUserAsync(request.UserId, cancellationToken: cancellationToken);
    }
}
