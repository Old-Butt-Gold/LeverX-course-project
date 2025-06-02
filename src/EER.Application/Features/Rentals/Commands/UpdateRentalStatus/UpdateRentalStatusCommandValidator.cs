using FluentValidation;

namespace EER.Application.Features.Rentals.Commands.UpdateRentalStatus;

public class UpdateRentalStatusCommandValidator : AbstractValidator<UpdateRentalStatusCommand>
{
    public UpdateRentalStatusCommandValidator()
    {
        RuleFor(x => x.UpdateRentalDto.Id)
            .GreaterThan(0).WithMessage("Invalid rental ID");

        RuleFor(x => x.UpdateRentalDto.Status)
            .IsInEnum();
    }
}
