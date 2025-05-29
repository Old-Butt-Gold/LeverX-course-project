using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Commands.UpdateOffice;

public record UpdateOfficeCommand(
    int Id,
    Office Office) : IRequest<Office>;
