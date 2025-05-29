using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Commands.CreateOffice;

internal sealed class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, Office>
{
    private readonly IOfficeRepository _repository;

    public CreateOfficeCommandHandler(IOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Office> Handle(CreateOfficeCommand command, CancellationToken cancellationToken)
    {
        // TODO UpdatedBy
        var office = new Office
        {
            OwnerId = command.OwnerId,
            Address = command.Address,
            City = command.City,
            Country = command.Country,
            IsActive = command.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
        };

        return await _repository.AddAsync(office, cancellationToken);
    }
}
