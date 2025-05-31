using EER.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EER.Application.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureMediatR(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
    }

    public static void ConfigureAutoMapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(AssemblyReference).Assembly);
    }

    public static void ConfigureFluentValidation(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssemblyContaining(typeof(AssemblyReference));
    }
}
