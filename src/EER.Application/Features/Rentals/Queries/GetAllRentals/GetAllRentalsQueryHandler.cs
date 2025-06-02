using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetAllRentals;

internal sealed class GetAllRentalsQueryHandler
    : IRequestHandler<GetAllRentalsQuery, IEnumerable<RentalDto>>
{
    private readonly IRentalRepository _repository;
    private readonly IMapper _mapper;

    public GetAllRentalsQueryHandler(IRentalRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RentalDto>> Handle(GetAllRentalsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<RentalDto>>(rentals);
    }
}
