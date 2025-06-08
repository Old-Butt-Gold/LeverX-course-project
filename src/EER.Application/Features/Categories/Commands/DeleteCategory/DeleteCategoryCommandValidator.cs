using EER.Domain.DatabaseAbstractions;
using FluentValidation;

namespace EER.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Id)
            .MustAsync(async (id, ct) =>
            {
                var category = await categoryRepository.GetByIdAsync(id, cancellationToken: ct);
                return category is null || category.TotalEquipment == 0;
            })
            .WithMessage("You can't delete category with more than one equipment referenced to it");
    }
}
