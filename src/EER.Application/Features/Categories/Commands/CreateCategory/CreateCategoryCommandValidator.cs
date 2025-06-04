using EER.Domain.DatabaseAbstractions;
using FluentValidation;

namespace EER.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.CreateCategoryDto.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CreateCategoryDto.Description)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.CreateCategoryDto.Slug)
            .NotEmpty()
            .Length(3, 100)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers and hyphens")
            .MustAsync(async (slug, ct) =>
                    !await categoryRepository.IsSlugExistsAsync(slug, cancellationToken: ct))
            .WithMessage("Slug already exists");
    }
}
