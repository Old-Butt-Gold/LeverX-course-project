using EER.Persistence.MongoDB.Documents.Category;
using EER.Persistence.MongoDB.Documents.Equipment;
using EER.Persistence.MongoDB.Documents.EquipmentItem;
using EER.Persistence.MongoDB.Documents.Office;
using EER.Persistence.MongoDB.Documents.Rental;
using EER.Persistence.MongoDB.Documents.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace EER.Persistence.MongoDB;

public static class MongoMappings
{
    public static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(UserDocument)))
        {
            BsonClassMap.RegisterClassMap<UserDocument>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(GuidGenerator.Instance)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

                cm.UnmapMember(u => u.OfficeIds);
                cm.UnmapMember(u => u.Favorites);

                cm.MapMember(u => u.OfficeIds)
                    .SetElementName("officeIds")
                    .SetSerializer(new ArraySerializer<int>(new Int32Serializer(BsonType.Int32)));

                cm.MapMember(u => u.Favorites)
                    .SetElementName("favorites")
                    .SetSerializer(new ArraySerializer<UserFavoriteEmbedded>());
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(CategoryDocument)))
        {
            BsonClassMap.RegisterClassMap<CategoryDocument>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(new Int32Serializer(BsonType.Int32));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(EquipmentDocument)))
        {
            BsonClassMap.RegisterClassMap<EquipmentDocument>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(new Int32Serializer(BsonType.Int32));

                cm.MapMember(e => e.Images)
                    .SetElementName("media")
                    .SetSerializer(new ArraySerializer<EquipmentImageEmbedded>());

                cm.MapMember(e => e.Reviews)
                    .SetElementName("reviews")
                    .SetSerializer(new ArraySerializer<ReviewEmbedded>());
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(EquipmentItemDocument)))
        {
            BsonClassMap.RegisterClassMap<EquipmentItemDocument>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetSerializer(new Int64Serializer(BsonType.Int64));

                cm.MapMember(i => i.MaintenanceDate)
                    .SetSerializer(new DateOnlySerializer(BsonType.DateTime, DateOnlyDocumentFormat.YearMonthDay));

                cm.MapMember(i => i.PurchaseDate)
                    .SetSerializer(new DateOnlySerializer(BsonType.DateTime, DateOnlyDocumentFormat.YearMonthDay));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(OfficeDocument)))
        {
            BsonClassMap.RegisterClassMap<OfficeDocument>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(new Int32Serializer(BsonType.Int32));

                cm.MapMember(o => o.EquipmentItemIds)
                    .SetElementName("equipmentItemIds")
                    .SetSerializer(new ArraySerializer<long>(new Int64Serializer(BsonType.Int64)));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(RentalDocument)))
        {
            BsonClassMap.RegisterClassMap<RentalDocument>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(new Int32Serializer(BsonType.Int32));

                cm.MapMember(r => r.Items)
                    .SetElementName("items")
                    .SetSerializer(new ArraySerializer<RentalItemEmbedded>());

                cm.MapMember(r => r.StartDate)
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));

                cm.MapMember(r => r.EndDate)
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            });
        }
    }
}
