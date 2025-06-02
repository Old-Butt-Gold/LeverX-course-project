namespace EER.Application.Features.Categories.Commands.CreateCategory;

public record CategoryCreatedDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid CreatedBy { get; init; }
}
