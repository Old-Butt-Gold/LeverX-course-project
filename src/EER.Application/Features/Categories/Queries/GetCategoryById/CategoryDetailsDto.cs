namespace EER.Application.Features.Categories.Queries.GetCategoryById;

public record CategoryDetailsDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Slug { get; init; }
    public int TotalEquipment { get; init; }
}
