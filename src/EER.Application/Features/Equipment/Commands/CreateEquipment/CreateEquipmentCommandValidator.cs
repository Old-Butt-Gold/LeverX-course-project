using FluentValidation;

namespace EER.Application.Features.Equipment.Commands.CreateEquipment;

public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
        RuleFor(x => x.CreateEquipmentDto.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CreateEquipmentDto.CategoryId)
            .GreaterThan(0).WithMessage("Invalid category ID");

        RuleFor(x => x.CreateEquipmentDto.Description)
            .NotEmpty()
            .MaximumLength(3000);

        RuleFor(x => x.CreateEquipmentDto.PricePerDay)
            .GreaterThan(0).WithMessage("Price per day must be greater than 0")
            .LessThanOrEqualTo(99999.99m).WithMessage("Price per day cannot exceed 99999.99");
    }
}
