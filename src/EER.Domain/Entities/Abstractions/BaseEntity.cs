namespace EER.Domain.Entities.Abstractions;

public abstract class BaseEntity<TKey> where TKey : struct
{
    public virtual TKey Id { get; set; }
}