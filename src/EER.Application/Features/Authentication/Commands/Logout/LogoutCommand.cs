using MediatR;

namespace EER.Application.Features.Authentication.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
