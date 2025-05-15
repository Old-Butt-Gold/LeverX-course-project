using EER.Domain.Entities.Abstractions;
using EER.Domain.Exceptions;

namespace EER.Domain.Entities;

public class Equipment : BaseAuditEntity<long>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal PricePerDay { get; set; }

    // add .Ignore in EF Core Configuration
    public bool IsAvailable => AvailableQuantity > 0;
    
    public int AvailableQuantity { get; private set; } 
    
    public string Location { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
    
    public ICollection<Rental> Rentals { get; set; } = [];

    public void Rent(int quantity)
    {
        if (quantity > AvailableQuantity)
            throw new DomainRuleException("Not enough equipment available.");
        
        AvailableQuantity -= quantity;
    }
    
    public void ReturnEquipment(int quantity)
    {
        AvailableQuantity += quantity;
    }
    
    // needs to be calculated on the database side later
    public bool IsAvailableBetween(DateTime start, DateTime end)
    {
        var overlappingRentals = Rentals
            .Where(r => !(end < r.StartDate || start > r.EndDate))
            .Sum(r => r.Quantity);
    
        return AvailableQuantity - overlappingRentals >= 0;
    }
}