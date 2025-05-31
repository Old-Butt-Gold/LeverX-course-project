namespace EER.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Slug { get; init; }
}
