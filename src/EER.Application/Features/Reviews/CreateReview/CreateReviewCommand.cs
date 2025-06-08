using MediatR;

namespace EER.Application.Features.Reviews.CreateReview;

public record CreateReviewCommand(CreateReviewDto ReviewDto, int EquipmentId, Guid CustomerId)
    : IRequest<ReviewCreatedDto>;
