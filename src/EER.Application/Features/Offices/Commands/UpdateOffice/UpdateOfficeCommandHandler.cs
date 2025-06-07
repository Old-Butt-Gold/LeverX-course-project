using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EER.Application.Features.Offices.Commands.UpdateOffice;

internal sealed class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, OfficeUpdatedDto>
{
    private readonly IOfficeRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOfficeCommandHandler> _logger;

    public UpdateOfficeCommandHandler(IOfficeRepository repository, IMapper mapper, ILogger<UpdateOfficeCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OfficeUpdatedDto> Handle(UpdateOfficeCommand command, CancellationToken cancellationToken)
    {
        var updatedDto = command.UpdateOfficeDto;

        var office = await _repository.GetByIdAsync(updatedDto.Id, cancellationToken: cancellationToken);

        if (office is null)
            throw new KeyNotFoundException($"Office with ID {updatedDto.Id} not found");

        if (office.OwnerId != command.Manipulator)
        {
            _logger.LogInformation("User with {userId} tried to update office with id {equipmentId} of Owner {ownerId}",
                command.Manipulator, office.Id, office.OwnerId);

            throw new UnauthorizedAccessException("You have no access to update this office");
        }

        var mappedOffice = _mapper.Map(updatedDto, office);
        office.UpdatedBy = command.Manipulator;

        var updatedOffice = await _repository.UpdateAsync(mappedOffice, cancellationToken: cancellationToken);

        return _mapper.Map<OfficeUpdatedDto>(updatedOffice);
    }
}
