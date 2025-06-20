﻿using AutoMapper;
using EER.API.Middleware;
using EER.Domain.DatabaseAbstractions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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

    public static void UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes = {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });
    }

    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
