using System.Data.Common;
using EER.Domain.DatabaseAbstractions;
using EER.Persistence.Dapper;
using EER.Persistence.EFCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EER.Persistence.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureMigrationService(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddSingleton<ISqlMigrationService, MssqlSqlMigrationService>();
    }

    public static void ConfigureDapper(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<DbConnection>(_ =>
            new SqlConnection(configuration.GetConnectionString("MSConnection")));

        serviceCollection.AddScoped<IUnitOfWork, DapperUnitOfWork>();
    }

    public static void ConfigureEntityFrameworkCore(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MSConnection")));

        // serviceCollection.AddScoped<IUnitOfWork, EntityFrameworkCoreUnitOfWork>();
    }
}
