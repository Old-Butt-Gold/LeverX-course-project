using FluentValidation;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.CreateRentalDto.EquipmentItemIds)
            .NotNull()
            .WithMessage("Equipment item list must be provided")
            .NotEmpty()
            .WithMessage("At least one equipment item ID must be specified");

        RuleForEach(x => x.CreateRentalDto.EquipmentItemIds)
            .GreaterThan(0)
            .WithMessage("Each equipment item ID must be greater than zero");

        RuleFor(x => x.CreateRentalDto.OwnerId)
            .NotEmpty().WithMessage("Owner ID is required");

        RuleFor(x => x.CreateRentalDto.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past");

        RuleFor(x => x.CreateRentalDto.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .Must((command, endDate) => endDate.Date > command.CreateRentalDto.StartDate.Date)
            .WithMessage("End date must be at least 1 day after start date");
    }
}
