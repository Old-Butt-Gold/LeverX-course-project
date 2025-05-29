namespace EER.Persistence.MongoDB.Documents;

public class SequenceDocument
{
    public string Id { get; set; } = null!;
    public int Value { get; set; }
    public long LongValue { get; set; }
}
