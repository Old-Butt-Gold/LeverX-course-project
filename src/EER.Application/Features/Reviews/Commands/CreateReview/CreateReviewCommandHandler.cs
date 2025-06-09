using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.Entities;
using MediatR;

namespace EER.Application.Features.Reviews.Commands.CreateReview;

internal sealed class CreateReviewCommandHandler
    : IRequestHandler<CreateReviewCommand, ReviewCreatedDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewCreatedDto> Handle(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        var review = _mapper.Map<Review>(command.ReviewDto);

        review.EquipmentId = command.EquipmentId;
        review.CustomerId = command.CustomerId;
        review.CreatedBy = command.CustomerId;
        review.UpdatedBy = command.CustomerId;

        var createdReview = await _reviewRepository.AddAsync(review, cancellationToken: cancellationToken);

        return _mapper.Map<ReviewCreatedDto>(createdReview);
    }
}
