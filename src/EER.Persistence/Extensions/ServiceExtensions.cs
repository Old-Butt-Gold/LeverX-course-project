using EER.Domain.DatabaseAbstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EER.Persistence.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureMigrationService(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddSingleton<ISqlMigrationService, MssqlSqlMigrationService>();
    }
}
