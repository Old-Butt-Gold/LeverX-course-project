﻿using System.Reflection;
using Microsoft.OpenApi.Models;

namespace EER.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureSwaggerGen(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Version = "v1",
                Title = "Event Equipment Rental API",
                Contact = new OpenApiContact
                {
                    Name = "Andrey",
                    Url = new("https://github.com/Old-Butt-Gold"),
                    Email = "andrey2004andrey2021@gmail.com",
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                }
            });
            
            var basePath = AppContext.BaseDirectory;
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(basePath, xmlFile);
            options.IncludeXmlComments(xmlPath);
            
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer <token>\"",
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
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                    },
                    []
                }
            });
        });
    }
}