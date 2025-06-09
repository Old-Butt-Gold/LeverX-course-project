namespace EER.Application.Features.Rentals.Queries.GetDetailedRental;

public record DetailedEquipmentItemDto
{
    public long EquipmentItemId { get; init; }
    public string SerialNumber { get; init; } = null!;
    public decimal ActualPrice { get; init; }
    public string EquipmentName { get; init; } = null!;
    public DateTime PurchaseDate { get; init; }
}
