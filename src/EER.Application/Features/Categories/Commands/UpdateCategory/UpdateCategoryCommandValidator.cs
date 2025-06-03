using EER.Domain.DatabaseAbstractions;
using FluentValidation;

namespace EER.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.UpdateCategoryDto.Id)
            .GreaterThan(0).WithMessage("Invalid category ID");

        RuleFor(x => x.UpdateCategoryDto.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UpdateCategoryDto.Description)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.UpdateCategoryDto.Slug)
            .NotEmpty()
            .Length(3, 100)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers and hyphens")
            .MustAsync(async (slug, ct) =>
                !await categoryRepository.IsSlugExists(slug, cancellationToken: ct))
            .WithMessage("Slug already exists");
    }
}
