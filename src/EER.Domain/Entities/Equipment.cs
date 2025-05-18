using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Equipment : BaseEntity<long>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public decimal PricePerDay { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public int TotalStock { get; set; } // how many items are in all without rental quantity
    public int AvailableQuantity { get; set; }
    
    // for denormalization
    public double AverageRating { get; set; }
    public long TotalReviews { get; set; }
    
    public int CategoryId { get; set; }
    
    public Guid OwnerId { get; set; }
}