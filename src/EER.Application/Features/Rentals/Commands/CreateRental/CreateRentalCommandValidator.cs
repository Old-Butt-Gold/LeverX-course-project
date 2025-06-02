using FluentValidation;

namespace EER.Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.CreateRentalDto.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

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
