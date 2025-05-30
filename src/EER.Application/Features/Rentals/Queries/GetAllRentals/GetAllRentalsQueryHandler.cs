using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetAllRentals;

internal sealed class GetAllRentalsQueryHandler : IRequestHandler<GetAllRentalsQuery, IEnumerable<Rental>>
{
    private readonly IRentalRepository _repository;

    public GetAllRentalsQueryHandler(IRentalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Rental>> Handle(GetAllRentalsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
