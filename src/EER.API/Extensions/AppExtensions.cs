using AutoMapper;
using EER.Domain.DatabaseAbstractions;
using EER.Persistence.Migrations;

namespace EER.API.Extensions;

public static class AppExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app, IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbMigrationService = scope.ServiceProvider.GetRequiredService<ISqlMigrationService>();

        dbMigrationService.ApplyMigrations();
    }

    public static void AssertAutoMapperConfigurationValid(this IApplicationBuilder app, IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var configProvider = mapper.ConfigurationProvider;

        configProvider.AssertConfigurationIsValid();
    }
}
