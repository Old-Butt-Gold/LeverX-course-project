using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetRentalById;

internal sealed class GetRentalByIdQueryHandler
    : IRequestHandler<GetRentalByIdQuery, RentalDetailsDto?>
{
    private readonly IRentalRepository _repository;
    private readonly IMapper _mapper;

    public GetRentalByIdQueryHandler(IRentalRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RentalDetailsDto?> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);

        return rental is null
            ? null
            : _mapper.Map<RentalDetailsDto>(rental);
    }
}
