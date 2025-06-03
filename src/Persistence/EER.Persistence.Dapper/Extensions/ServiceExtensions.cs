using System.Data.Common;
using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Persistence.Dapper.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EER.Persistence.Dapper.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDapper(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<DbConnection>(_ =>
            new SqlConnection(configuration.GetConnectionString("MSConnection")));

        serviceCollection.AddScoped<ICategoryRepository, DapperCategoryRepository>();
        serviceCollection.AddScoped<IEquipmentItemRepository, DapperEquipmentItemRepository>();
        serviceCollection.AddScoped<IEquipmentRepository, DapperEquipmentRepository>();
        serviceCollection.AddScoped<IOfficeRepository, DapperOfficeRepository>();
        serviceCollection.AddScoped<IRentalRepository, DapperRentalRepository>();
        serviceCollection.AddScoped<IUserRepository, DapperUserRepository>();
        serviceCollection.AddScoped<IRefreshTokenRepository, DapperRefreshTokenRepository>();

        serviceCollection.AddScoped<ITransactionManager, DapperTransactionManager>();
    }
}
