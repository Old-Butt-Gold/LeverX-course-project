using EER.Domain.DatabaseAbstractions;
using FluentValidation;

namespace EER.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator(IReviewRepository repository)
    {
        RuleFor(x => x.CustomerId)
            .MustAsync(async (command, _, ct) =>
                !await repository.IsExistsReview(command.CustomerId, command.EquipmentId, ct: ct))
            .WithMessage("You are already written a review");

        RuleFor(x => x.EquipmentId)
            .GreaterThan(0).WithMessage("Invalid equipment ID");

        RuleFor(x => x.ReviewDto.Rating)
            .InclusiveBetween((byte)1, (byte)5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.ReviewDto.Comment)
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
    }
}
