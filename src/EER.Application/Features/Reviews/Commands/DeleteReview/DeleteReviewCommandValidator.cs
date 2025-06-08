using FluentValidation;

namespace EER.Application.Features.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.EquipmentId)
            .GreaterThan(0).WithMessage("Invalid equipment ID");
            
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");
    }
}
