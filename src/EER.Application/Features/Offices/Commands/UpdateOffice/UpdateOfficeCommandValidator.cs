using FluentValidation;

namespace EER.Application.Features.Offices.Commands.UpdateOffice;

public class UpdateOfficeCommandValidator : AbstractValidator<UpdateOfficeCommand>
{
    public UpdateOfficeCommandValidator()
    {
        RuleFor(x => x.UpdateOfficeDto.Id)
            .GreaterThan(0).WithMessage("Invalid office ID");

        RuleFor(x => x.UpdateOfficeDto.OwnerId)
            .NotEmpty();

        RuleFor(x => x.UpdateOfficeDto.Address)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.UpdateOfficeDto.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UpdateOfficeDto.Country)
            .NotEmpty()
            .MaximumLength(100);
    }
}
