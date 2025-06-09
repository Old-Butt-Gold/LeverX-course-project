using FluentValidation;

namespace EER.Application.Features.EquipmentItems.Commands.CreateEquipmentItem;

public class CreateEquipmentItemCommandValidator : AbstractValidator<CreateEquipmentItemCommand>
{
    public CreateEquipmentItemCommandValidator()
    {
        RuleFor(x => x.CreateEquipmentItemDto.EquipmentId)
            .GreaterThan(0).WithMessage("Invalid equipment ID");

        RuleFor(x => x.CreateEquipmentItemDto.SerialNumber)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Serial number cannot exceed 100 characters");

        RuleFor(x => x.CreateEquipmentItemDto.ItemStatus)
            .IsInEnum();

        RuleFor(x => x.CreateEquipmentItemDto.OfficeId)
            .GreaterThan(0).WithMessage("Invalid office ID")
            .When(x => x.CreateEquipmentItemDto.OfficeId.HasValue);

        RuleFor(x => x.CreateEquipmentItemDto.PurchaseDate)
            .NotEmpty().WithMessage("Purchase date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Purchase date cannot be in the future");

        RuleFor(x => x.CreateEquipmentItemDto.MaintenanceDate)
            .GreaterThanOrEqualTo(x => x.CreateEquipmentItemDto.PurchaseDate)
            .When(x => x.CreateEquipmentItemDto.MaintenanceDate.HasValue)
            .WithMessage("Maintenance date must be after purchase date");
    }
}
