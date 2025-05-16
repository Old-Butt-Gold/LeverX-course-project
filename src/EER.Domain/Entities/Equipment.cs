using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Equipment : BaseEntity<long>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal PricePerDay { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool IsAvailable => AvailableQuantity > 0;
    
    public int AvailableQuantity { get; set; } 
    
    public string Location { get; set; }
    
    public int CategoryId { get; set; }
    public Guid OwnerId { get; set; }
}