using MediatR;

namespace EER.Application.Features.Offices.Commands.UpdateOffice;

public record UpdateOfficeCommand(UpdateOfficeDto UpdateOfficeDto) : IRequest<OfficeUpdatedDto>;
