using MediatR;

namespace EER.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(RefreshTokenDto RefreshTokenDto) : IRequest<RefreshTokenResultDto>;
