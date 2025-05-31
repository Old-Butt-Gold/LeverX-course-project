namespace EER.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Slug { get; init; }
}
