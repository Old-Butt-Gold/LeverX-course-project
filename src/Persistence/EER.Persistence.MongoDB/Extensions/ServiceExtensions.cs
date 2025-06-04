using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Persistence.MongoDB.Repositories;
using EER.Persistence.MongoDB.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EER.Persistence.MongoDB.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureMongo(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<DatabaseSettings>(configuration.GetSection("Mongo"));

        serviceCollection.AddSingleton<IMongoClient>(sp =>
        {
            var dbSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            return new MongoClient(dbSettings.ConnectionString);
        });

        serviceCollection.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var dbSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            return client.GetDatabase(dbSettings.DatabaseName);
        });

        serviceCollection.AddScoped<IdGenerator>();
        serviceCollection.AddScoped<MongoDbInitializer>();

        serviceCollection.AddScoped<IUserRepository, MongoUserRepository>();
        serviceCollection.AddScoped<IOfficeRepository, MongoOfficeRepository>();
        serviceCollection.AddScoped<IRentalRepository, MongoRentalRepository>();
        serviceCollection.AddScoped<IEquipmentRepository, MongoEquipmentRepository>();
        serviceCollection.AddScoped<IEquipmentItemRepository, MongoEquipmentItemRepository>();
        serviceCollection.AddScoped<ICategoryRepository, MongoCategoryRepository>();
        serviceCollection.AddScoped<IRefreshTokenRepository, MongoRefreshTokenRepository>();

        serviceCollection.AddScoped<ITransactionManager, MongoTransactionManager>();

        MongoDbMappings.RegisterClassMaps();
    }

    public static async Task InitializeMongoDb(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();
        await initializer.InitializeAsync();
    }
}
