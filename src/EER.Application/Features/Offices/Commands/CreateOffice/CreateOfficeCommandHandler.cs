using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Offices.Commands.CreateOffice;

internal sealed class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, OfficeCreatedDto>
{
    private readonly IOfficeRepository _repository;
    private readonly IMapper _mapper;

    public CreateOfficeCommandHandler(IOfficeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<OfficeCreatedDto> Handle(CreateOfficeCommand command, CancellationToken cancellationToken)
    {
        var office = _mapper.Map<Office>(command.CreateOfficeDto);

        var createdOffice = await _repository.AddAsync(office, cancellationToken);

        return _mapper.Map<OfficeCreatedDto>(createdOffice);
    }
}
