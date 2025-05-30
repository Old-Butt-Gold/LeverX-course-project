using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Rentals.Queries.GetRentalById;

internal sealed class GetRentalByIdQueryHandler : IRequestHandler<GetRentalByIdQuery, Rental?>
{
    private readonly IRentalRepository _repository;

    public GetRentalByIdQueryHandler(IRentalRepository repository)
    {
        _repository = repository;
    }

    public async Task<Rental?> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
