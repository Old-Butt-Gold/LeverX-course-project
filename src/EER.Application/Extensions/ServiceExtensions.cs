using Microsoft.Extensions.DependencyInjection;

namespace EER.Application.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureMediatR(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
        });
    }
}
