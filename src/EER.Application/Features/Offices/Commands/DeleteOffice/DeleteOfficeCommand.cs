using MediatR;

namespace EER.Application.Features.Offices.Commands.DeleteOffice;

public record DeleteOfficeCommand(int Id, Guid Manipulator) : IRequest<bool>;
