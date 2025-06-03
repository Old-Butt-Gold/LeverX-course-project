using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Offices.Commands.DeleteOffice;

internal sealed class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand, bool>
{
    private readonly IOfficeRepository _repository;

    public DeleteOfficeCommandHandler(IOfficeRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteOfficeCommand command, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(command.Id, cancellationToken: cancellationToken);
    }
}
