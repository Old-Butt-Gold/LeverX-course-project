using MediatR;

namespace EER.Application.Features.Authentication.Commands.LogoutAll;

public record LogoutAllCommand(Guid UserId) : IRequest;
