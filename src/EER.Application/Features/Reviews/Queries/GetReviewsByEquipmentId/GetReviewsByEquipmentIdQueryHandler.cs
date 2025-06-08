using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using MediatR;

namespace EER.Application.Features.Reviews.Queries.GetReviewsByEquipmentId;

internal sealed class GetReviewsByEquipmentIdQueryHandler
    : IRequestHandler<GetReviewsByEquipmentIdQuery, IEnumerable<ReviewWithUserDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewsByEquipmentIdQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewWithUserDto>> Handle(GetReviewsByEquipmentIdQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository
            .GetReviewsByEquipmentIdAsync(request.EquipmentId, ct: cancellationToken);

        return _mapper.Map<IEnumerable<ReviewWithUserDto>>(reviews);
    }
}
