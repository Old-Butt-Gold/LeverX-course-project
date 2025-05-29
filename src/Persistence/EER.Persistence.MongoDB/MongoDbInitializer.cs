using EER.Persistence.MongoDB.Documents;
using EER.Persistence.MongoDB.Documents.Category;
using EER.Persistence.MongoDB.Documents.Equipment;
using EER.Persistence.MongoDB.Documents.EquipmentItem;
using EER.Persistence.MongoDB.Documents.Office;
using EER.Persistence.MongoDB.Documents.Rental;
using EER.Persistence.MongoDB.Documents.User;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB;

public class MongoDbInitializer
{
    private readonly IMongoDatabase _database;
    private readonly DatabaseSettings _settings;

    public MongoDbInitializer(IMongoDatabase database, IOptions<DatabaseSettings> settings)
    {
        _database = database;
        _settings = settings.Value;
    }

    public async Task InitializeAsync()
    {
        await CreateIndexes();
        await InitializeSequences();
    }

    private async Task CreateIndexes()
    {
        var users = _database.GetCollection<UserDocument>(_settings.UserCollection);
        await users.Indexes.CreateManyAsync([
            new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true, }
            ),
            new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending(u => u.UserRole)),
            new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending("Favorites.EquipmentId")),
            new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending("Favorites.CreatedAt"))
        ]);

        var equipment = _database.GetCollection<EquipmentDocument>(_settings.EquipmentCollection);
        await equipment.Indexes.CreateManyAsync([
            new CreateIndexModel<EquipmentDocument>(
                Builders<EquipmentDocument>.IndexKeys.Ascending(e => e.CategoryId)),
            new CreateIndexModel<EquipmentDocument>(
                Builders<EquipmentDocument>.IndexKeys.Ascending(e => e.OwnerId)),
            new CreateIndexModel<EquipmentDocument>(
                Builders<EquipmentDocument>.IndexKeys.Text(e => e.Name)),
            new CreateIndexModel<EquipmentDocument>(
                Builders<EquipmentDocument>.IndexKeys.Ascending("Reviews.CustomerId"))
        ]);

        var items = _database.GetCollection<EquipmentItemDocument>(_settings.EquipmentItemCollection);
        await items.Indexes.CreateManyAsync([
            new CreateIndexModel<EquipmentItemDocument>(
                Builders<EquipmentItemDocument>.IndexKeys.Combine(
                    Builders<EquipmentItemDocument>.IndexKeys.Ascending(i => i.EquipmentId),
                    Builders<EquipmentItemDocument>.IndexKeys.Ascending(i => i.Status))),
            new CreateIndexModel<EquipmentItemDocument>(
                Builders<EquipmentItemDocument>.IndexKeys.Ascending(i => i.OfficeId)),
            new CreateIndexModel<EquipmentItemDocument>(
                Builders<EquipmentItemDocument>.IndexKeys.Combine(
                    Builders<EquipmentItemDocument>.IndexKeys.Ascending(i => i.SerialNumber),
                    Builders<EquipmentItemDocument>.IndexKeys.Ascending(i => i.EquipmentId)),
                new CreateIndexOptions { Unique = true })
        ]);

        var rentals = _database.GetCollection<RentalDocument>(_settings.RentalCollection);
        await rentals.Indexes.CreateManyAsync([
            new CreateIndexModel<RentalDocument>(
                Builders<RentalDocument>.IndexKeys.Combine(
                    Builders<RentalDocument>.IndexKeys.Ascending(r => r.StartDate),
                    Builders<RentalDocument>.IndexKeys.Ascending(r => r.EndDate))),
            new CreateIndexModel<RentalDocument>(
                Builders<RentalDocument>.IndexKeys.Ascending(r => r.CustomerId)),
            new CreateIndexModel<RentalDocument>(
                Builders<RentalDocument>.IndexKeys.Combine(
                    Builders<RentalDocument>.IndexKeys.Ascending(r => r.OwnerId),
                    Builders<RentalDocument>.IndexKeys.Ascending(r => r.Status),
                    Builders<RentalDocument>.IndexKeys.Ascending(r => r.CreatedAt)
                )
            )
        ]);

        var offices = _database.GetCollection<OfficeDocument>(_settings.OfficeCollection);
        await offices.Indexes.CreateManyAsync([
                new CreateIndexModel<OfficeDocument>(
                    Builders<OfficeDocument>.IndexKeys.Combine(
                        Builders<OfficeDocument>.IndexKeys.Ascending(o => o.City),
                        Builders<OfficeDocument>.IndexKeys.Ascending(o => o.Country))),
                new CreateIndexModel<OfficeDocument>(
                    Builders<OfficeDocument>.IndexKeys.Ascending(o => o.OwnerId)
                )
            ]
        );

        var categories = _database.GetCollection<CategoryDocument>(_settings.CategoryCollection);
        await categories.Indexes.CreateOneAsync(
            new CreateIndexModel<CategoryDocument>(
                Builders<CategoryDocument>.IndexKeys.Ascending(c => c.Slug),
                new CreateIndexOptions { Unique = true })
        );
    }

    private async Task InitializeSequences()
    {
        var sequences = new List<(string name, int start, long longStart)>
        {
            (_settings.CategoryCollection, 1, 0),
            (_settings.EquipmentCollection, 1, 0),
            (_settings.OfficeCollection, 1, 0),
            (_settings.RentalCollection, 1, 0),
            (_settings.EquipmentItemCollection, 0, 1),
            (_settings.ImagesEmbedded, 1, 0)
        };

        var collection = _database.GetCollection<SequenceDocument>("sequences");
        foreach (var (name, start, longStart) in sequences)
        {
            var filter = Builders<SequenceDocument>.Filter.Eq(s => s.Id, name);
            var existing = await collection.Find(filter).FirstOrDefaultAsync();

            if (existing is null)
            {
                await collection.InsertOneAsync(
                    new SequenceDocument { Id = name, Value = start, LongValue = longStart });
            }
        }
    }
}
