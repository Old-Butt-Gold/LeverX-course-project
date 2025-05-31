using FluentValidation;

namespace EER.Application.Features.Equipment.Commands.UpdateEquipment;

public class UpdateEquipmentCommandValidator : AbstractValidator<UpdateEquipmentCommand>
{
    public UpdateEquipmentCommandValidator()
    {
        RuleFor(x => x.UpdateEquipmentDto.Id)
            .GreaterThan(0).WithMessage("Invalid equipment ID");

        RuleFor(x => x.UpdateEquipmentDto.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UpdateEquipmentDto.CategoryId)
            .GreaterThan(0).WithMessage("Invalid category ID");

        RuleFor(x => x.UpdateEquipmentDto.Description)
            .NotEmpty()
            .MaximumLength(3000);

        RuleFor(x => x.UpdateEquipmentDto.PricePerDay)
            .GreaterThan(0).WithMessage("Price per day must be greater than 0")
            .LessThanOrEqualTo(99999.99m).WithMessage("Price per day cannot exceed 99999.99");
    }
}
