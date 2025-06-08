using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetMyRentals;

public class GetMyRentalsHandler : IRequestHandler<GetMyRentalsQuery, IEnumerable<MyRentalDto>>
{
    private readonly IRentalRepository _repository;
    private readonly IMapper _mapper;

    public GetMyRentalsHandler(IRentalRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MyRentalDto>> Handle(GetMyRentalsQuery request, CancellationToken ct)
    {
        var rentals = await _repository.GetByUserIdAsync(request.UserId, request.UserRole, cancellationToken: ct);

        return _mapper.Map<IEnumerable<MyRentalDto>>(rentals);
    }
}
