using FluentValidation;

namespace EER.Application.Features.Offices.Commands.CreateOffice;

public class CreateOfficeCommandValidator : AbstractValidator<CreateOfficeCommand>
{
    public CreateOfficeCommandValidator()
    {
        RuleFor(x => x.CreateOfficeDto.OwnerId)
            .NotEmpty();

        RuleFor(x => x.CreateOfficeDto.Address)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.CreateOfficeDto.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CreateOfficeDto.Country)
            .NotEmpty()
            .MaximumLength(100);
    }
}
