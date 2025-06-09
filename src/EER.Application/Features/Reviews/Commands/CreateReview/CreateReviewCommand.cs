using MediatR;

namespace EER.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(CreateReviewDto ReviewDto, int EquipmentId, Guid CustomerId)
    : IRequest<ReviewCreatedDto>;
