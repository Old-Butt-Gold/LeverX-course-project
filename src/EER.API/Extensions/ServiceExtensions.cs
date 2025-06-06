using System.Diagnostics;
using System.Reflection;
using System.Text;
using EER.API.CustomAttributes;
using EER.API.Filters;
using EER.API.SwaggerSchemaFilters;
using EER.Application.Abstractions.Security;
using EER.Application.Services.Security;
using EER.Application.Settings;
using EER.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace EER.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;

            var assembly = typeof(Program).Assembly;

            var addHeaderAttributes = assembly.GetExportedTypes()
                .Where(x => x.GetCustomAttributes<AddHeaderAttribute>().Any());

            var requiredHeaderAttributes = assembly.GetExportedTypes()
                .Where(x => x.GetCustomAttributes<RequiredHeaderAttribute>().Any());

            if (addHeaderAttributes.Any())
            {
                config.Filters.Add<AddHeaderFilter>();
            }

            if (requiredHeaderAttributes.Any())
            {
                config.Filters.Add<RequiredHeaderFilter>();
            }

        }).AddXmlDataContractSerializerFormatters();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsGlobalPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    public static void ConfigureSwaggerGen(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Event Equipment Rental API",
                    Contact = new OpenApiContact
                    {
                        Name = "Andrey",
                        Url = new("https://github.com/Old-Butt-Gold"),
                        Email = "andrey2004andrey2021@gmail.com",
                    },
                    License = new OpenApiLicense { Name = "MIT License", }
                });

            var basePath = AppContext.BaseDirectory;
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(basePath, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Description = "JWT Authorization header using the Bearer scheme.\n\n" +
                              "In Swagger just add your Token.\n\n" +
                              "on other services enter 'Bearer' [space] and then your token.\n\n" +
                              "Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                BearerFormat = "JWT",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                    },
                    []
                }
            });

            options.SchemaFilter<EnumSchemaFilter>();
        });
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        var signingKey = configuration[$"{JwtSettings.Section}:SigningKey"];

        if (string.IsNullOrWhiteSpace(signingKey))
            throw new InvalidOperationException("JWT private key not configured");

        services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration[$"{JwtSettings.Section}:Issuer"],
                    ValidAudience = configuration[$"{JwtSettings.Section}:Audience"],
                    NameClaimType = JwtRegisteredClaimNames.Sid,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };
            });

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddAuthorizationBuilder()
            .AddPolicy("AnyRole", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Role.Admin.ToString(), Role.Customer.ToString(), Role.Owner.ToString());
            })
            .AddPolicy("AdminOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Role.Admin.ToString());
            })
            .AddPolicy("OwnerOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Role.Owner.ToString());
            })
            .AddPolicy("OwnerOrAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Role.Admin.ToString(), Role.Owner.ToString());
            })
            .AddPolicy("CustomerOrOwner", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Role.Customer.ToString(), Role.Owner.ToString());
            })
            .AddPolicy("CustomerOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Role.Customer.ToString());
            });

    }

    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var healthChecks = services.AddHealthChecks();

        var sqlConnectionString = configuration.GetConnectionString("MSConnection");
        if (!string.IsNullOrEmpty(sqlConnectionString))
        {
            healthChecks.AddSqlServer(
                connectionString: sqlConnectionString,
                name: "SQL Server",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "persistence"]
            );
        }
        else
        {
            healthChecks.AddCheck("SQL Server Configuration",
                () => HealthCheckResult.Unhealthy("SQL Server connection string is missing"),
                tags: ["db", "persistence"]);
        }

        var mongoConnectionString = configuration["Mongo:ConnectionString"];
        if (!string.IsNullOrEmpty(mongoConnectionString))
        {
            healthChecks.AddMongoDb(
                clientFactory: _ => new MongoClient(mongoConnectionString),
                name: "MongoDB",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "nosql", "persistence"]
            );
        }
        else
        {
            healthChecks.AddCheck("MongoDB Configuration",
                () => HealthCheckResult.Unhealthy("MongoDB connection string is missing"),
                tags: ["db", "nosql", "persistence"]);
        }

        const long oneGb = 1024 * 1024 * 1024;

        healthChecks.AddProcessAllocatedMemoryHealthCheck(1024,
                name: "Allocated Memory",
                failureStatus: HealthStatus.Degraded,
                tags: ["system", "memory"])
            .AddDiskStorageHealthCheck(opt =>
                {
                    opt.WithCheckAllDrives();
                },
                name: "Disk Storage",
                failureStatus: HealthStatus.Degraded,
                tags: ["system", "storage"])
            .AddCheck("Private Memory", () =>
            {
                var process = Process.GetCurrentProcess();
                var memory = process.PrivateMemorySize64;
                var status = memory < oneGb
                    ? HealthStatus.Healthy
                    : HealthStatus.Degraded;

                return new HealthCheckResult(
                    status,
                    description: $"Private memory: {memory / 1024 / 1024} MB",
                    data: new Dictionary<string, object> { ["limit"] = oneGb });
            }, tags: ["system", "memory"])
            .AddCheck("Working Set Memory", () =>
            {
                var process = Process.GetCurrentProcess();
                var memory = process.WorkingSet64;
                var status = memory < oneGb
                    ? HealthStatus.Healthy
                    : HealthStatus.Degraded;

                return new HealthCheckResult(
                    status,
                    description: $"Working set: {memory / 1024 / 1024} MB",
                    data: new Dictionary<string, object> { ["limit"] = oneGb });
            }, tags: ["system", "memory"]);
    }

    public static void ConfigureSerilog(this IServiceCollection services, IHostEnvironment hostEnvironment)
    {
        services.AddSerilog(config =>
        {
            config.MinimumLevel.Is(LogEventLevel.Verbose)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.Extensions.Hosting", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Information)
                .Enrich.FromLogContext();

            config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine} {Properties:j}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information, theme: AnsiConsoleTheme.Code);

            config.WriteTo.File(path: Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "logs"), "logfile-.txt"),
                rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
                restrictedToMinimumLevel: LogEventLevel.Information, fileSizeLimitBytes: 10 * 1024 * 1024,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level:u} | [{SourceContext}] | {Message:lj}{NewLine} {Properties:j}{NewLine}{Exception}");
        });
    }
}
