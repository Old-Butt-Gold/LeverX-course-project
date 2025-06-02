namespace EER.Application.Features.Categories.Queries.GetAllCategories;

public record CategoryDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public int TotalEquipment { get; set; }
}
