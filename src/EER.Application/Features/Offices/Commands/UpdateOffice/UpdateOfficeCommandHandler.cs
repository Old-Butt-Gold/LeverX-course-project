using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Offices.Commands.UpdateOffice;

internal sealed class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, OfficeUpdatedDto>
{
    private readonly IOfficeRepository _repository;
    private readonly IMapper _mapper;

    public UpdateOfficeCommandHandler(IOfficeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<OfficeUpdatedDto> Handle(UpdateOfficeCommand command, CancellationToken cancellationToken)
    {
        var updatedDto = command.UpdateOfficeDto;

        var office = await _repository.GetByIdAsync(updatedDto.Id, cancellationToken: cancellationToken);

        if (office is null)
            throw new KeyNotFoundException($"Office with ID {updatedDto.Id} not found");

        var mappedOffice = _mapper.Map(updatedDto, office);
        office.UpdatedBy = command.Manipulator;

        var updatedOffice = await _repository.UpdateAsync(mappedOffice, cancellationToken: cancellationToken);

        return _mapper.Map<OfficeUpdatedDto>(updatedOffice);
    }
}
