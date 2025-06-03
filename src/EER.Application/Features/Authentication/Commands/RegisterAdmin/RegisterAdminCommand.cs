using MediatR;

namespace EER.Application.Features.Authentication.Commands.RegisterAdmin;

public record RegisterAdminCommand(RegisterAdminDto AdminDto) : IRequest;
