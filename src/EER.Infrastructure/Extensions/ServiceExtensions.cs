using EER.Application.Abstractions.Security;
using EER.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace EER.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureSecurity(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
    }
}
