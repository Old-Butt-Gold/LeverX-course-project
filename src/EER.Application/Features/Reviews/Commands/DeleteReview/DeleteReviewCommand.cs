using MediatR;

namespace EER.Application.Features.Reviews.Commands.DeleteReview;

public record DeleteReviewCommand(int EquipmentId, Guid CustomerId) : IRequest<bool>;
