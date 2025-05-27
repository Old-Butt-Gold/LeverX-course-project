using EER.Application.Abstractions.Services;
using EER.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EER.Application.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddScoped<ICategoryService, CategoryService>();
        servicesCollection.AddScoped<IEquipmentService, EquipmentService>();
        servicesCollection.AddScoped<IEquipmentItemService, EquipmentItemService>();
        servicesCollection.AddScoped<IOfficeService, OfficeService>();
        servicesCollection.AddScoped<IRentalService, RentalService>();
        servicesCollection.AddScoped<IUserService, UserService>();
    }
}
