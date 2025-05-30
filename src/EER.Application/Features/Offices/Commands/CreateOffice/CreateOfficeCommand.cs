using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Commands.CreateOffice;

public record CreateOfficeCommand(
    Guid OwnerId,
    string Address,
    string City,
    string Country,
    bool IsActive) : IRequest<Office>;
