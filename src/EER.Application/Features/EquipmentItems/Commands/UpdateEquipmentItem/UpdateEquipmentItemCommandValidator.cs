using FluentValidation;

namespace EER.Application.Features.EquipmentItems.Commands.UpdateEquipmentItem;

public class UpdateEquipmentItemCommandValidator : AbstractValidator<UpdateEquipmentItemCommand>
{
    public UpdateEquipmentItemCommandValidator()
    {
        RuleFor(x => x.UpdateEquipmentItemDto.Id)
            .GreaterThan(0).WithMessage("Invalid item ID");

        RuleFor(x => x.UpdateEquipmentItemDto.SerialNumber)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UpdateEquipmentItemDto.ItemStatus)
            .IsInEnum();

        RuleFor(x => x.UpdateEquipmentItemDto.OfficeId)
            .GreaterThan(0).WithMessage("Invalid office ID")
            .When(x => x.UpdateEquipmentItemDto.OfficeId.HasValue);

        RuleFor(x => x.UpdateEquipmentItemDto.PurchaseDate)
            .NotEmpty().WithMessage("Purchase date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Purchase date cannot be in the future");

        RuleFor(x => x.UpdateEquipmentItemDto.MaintenanceDate)
            .GreaterThanOrEqualTo(x => x.UpdateEquipmentItemDto.PurchaseDate)
            .When(x => x.UpdateEquipmentItemDto.MaintenanceDate.HasValue)
            .WithMessage("Maintenance date must be after purchase date");
    }
}
