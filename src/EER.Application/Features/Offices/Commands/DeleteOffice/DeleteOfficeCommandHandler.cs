using EER.Domain.DatabaseAbstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EER.Application.Features.Offices.Commands.DeleteOffice;

internal sealed class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand, bool>
{
    private readonly IOfficeRepository _repository;
    private readonly ILogger<DeleteOfficeCommandHandler> _logger;

    public DeleteOfficeCommandHandler(IOfficeRepository repository, ILogger<DeleteOfficeCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteOfficeCommand command, CancellationToken cancellationToken)
    {
        var office = await _repository.GetByIdAsync(command.Id, cancellationToken: cancellationToken);

        if (office != null && office.OwnerId != command.Manipulator)
        {
            _logger.LogInformation("User with {userId} tried to delete office with id {officeId} of Owner {ownerId}",
                command.Manipulator, office.Id, office.OwnerId);

            throw new UnauthorizedAccessException("You have no access to update this equipment");
        }

        return await _repository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
    }
}
