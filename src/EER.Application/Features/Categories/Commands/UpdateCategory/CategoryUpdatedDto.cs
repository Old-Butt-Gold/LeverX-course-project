namespace EER.Application.Features.Categories.Commands.UpdateCategory;

public record CategoryUpdatedDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid UpdatedBy { get; init; }
}
