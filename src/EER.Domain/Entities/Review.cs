using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Review : BaseAuditEntity<long>
{
    public int Rating { get; set; }  // 1-5
    public string? Comment { get; set; }
    
    public Guid UserId { get; set; }
    public User Creator { get; set; }
    
    public long RentalId { get; set; }
    public Rental Rental { get; set; }
}