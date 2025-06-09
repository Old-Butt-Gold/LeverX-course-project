using MediatR;

namespace EER.Application.Features.Reviews.Queries.GetReviewsByEquipmentId;

public record GetReviewsByEquipmentIdQuery(int EquipmentId)
    : IRequest<IEnumerable<ReviewWithUserDto>>;
