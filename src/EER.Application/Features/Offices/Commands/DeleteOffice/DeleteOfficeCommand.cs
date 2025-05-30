using MediatR;

namespace EER.Application.Features.Offices.Commands.DeleteOffice;

public record DeleteOfficeCommand(int Id) : IRequest<bool>;
