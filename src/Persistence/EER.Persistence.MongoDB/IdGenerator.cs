using EER.Persistence.MongoDB.Documents;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB;

public class IdGenerator
{
    private readonly IMongoCollection<SequenceDocument> _sequenceCollection;

    public IdGenerator(IMongoDatabase database)
    {
        _sequenceCollection = database.GetCollection<SequenceDocument>("sequences");
    }

    public async Task<int> GetNextIdAsync(string sequenceName)
    {
        var filter = Builders<SequenceDocument>.Filter.Eq(s => s.Id, sequenceName);
        var update = Builders<SequenceDocument>.Update.Inc(s => s.Value, 1);
        var options = new FindOneAndUpdateOptions<SequenceDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var sequence = await _sequenceCollection.FindOneAndUpdateAsync(filter, update, options);
        return sequence.Value;
    }

    public async Task<long> GetNextLongIdAsync(string sequenceName)
    {
        var filter = Builders<SequenceDocument>.Filter.Eq(s => s.Id, sequenceName);
        var update = Builders<SequenceDocument>.Update.Inc(s => s.LongValue, 1);
        var options = new FindOneAndUpdateOptions<SequenceDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };

        var sequence = await _sequenceCollection.FindOneAndUpdateAsync(filter, update, options);
        return sequence.LongValue;
    }
}
