using EER.Domain.Entities.Abstractions;

namespace EER.Domain.Entities;

public class Category : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
}