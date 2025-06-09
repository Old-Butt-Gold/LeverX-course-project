using EER.Domain.DatabaseAbstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EER.Application.Features.Reviews.Commands.DeleteReview;

internal sealed class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<DeleteReviewCommandHandler> _logger;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository, ILogger<DeleteReviewCommandHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetReviewAsync(request.CustomerId, request.EquipmentId, ct: cancellationToken);

        if (review is not null && review.CustomerId != request.CustomerId)
        {
            _logger.LogInformation("User with {userId} tried to delete review with id {equipmentId}-{customerId} of Customer {customerId}",
                request.CustomerId, review.EquipmentId, request.CustomerId, review.CustomerId);

            throw new UnauthorizedAccessException("You have no access to delete this review");
        }

        return await _reviewRepository.DeleteReviewAsync(request.CustomerId, request.EquipmentId, cancellationToken: cancellationToken);
    }
}
