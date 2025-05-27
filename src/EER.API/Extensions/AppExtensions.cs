using EER.Domain.DatabaseAbstractions;
using EER.Persistence;

namespace EER.API.Extensions;

public static class AppExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app, IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbMigrationService = scope.ServiceProvider.GetRequiredService<ISqlMigrationService>();

        dbMigrationService.ApplyMigrations();
    }
}
