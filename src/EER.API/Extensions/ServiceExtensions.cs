using System.Reflection;
using EER.API.CustomAttributes;
using EER.API.Filters;
using EER.API.SwaggerSchemaFilters;
using Microsoft.OpenApi.Models;

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
                              "Enter 'Bearer' [space] and then your token.\n\n" +
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
}
