using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Commands.UpdateOffice;

internal sealed class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, Office>
{
    private readonly IOfficeRepository _repository;

    public UpdateOfficeCommandHandler(IOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Office> Handle(UpdateOfficeCommand command, CancellationToken cancellationToken)
    {
        var office = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (office is null)
            throw new KeyNotFoundException("Office with provided ID is not found");

        var updatedOffice = command.Office;

        office.Address = updatedOffice.Address;
        office.City = updatedOffice.City;
        office.Country = updatedOffice.Country;
        office.IsActive = updatedOffice.IsActive;
        office.UpdatedAt = DateTime.UtcNow;
        office.UpdatedBy = Guid.NewGuid();

        return await _repository.UpdateAsync(office, cancellationToken);
    }
}
