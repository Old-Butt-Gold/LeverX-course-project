using EER.Domain.DatabaseAbstractions;
using EER.Domain.DatabaseAbstractions.Transaction;
using EER.Persistence.EFCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EER.Persistence.EFCore.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureEntityFrameworkCore(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MSConnection")));

        serviceCollection.AddScoped<ICategoryRepository, EfCategoryRepository>();
        serviceCollection.AddScoped<IEquipmentItemRepository, EfEquipmentItemRepository>();
        serviceCollection.AddScoped<IEquipmentRepository, EfEquipmentRepository>();
        serviceCollection.AddScoped<IOfficeRepository, EfOfficeRepository>();
        serviceCollection.AddScoped<IRentalRepository, EfRentalRepository>();
        serviceCollection.AddScoped<IUserRepository, EfUserRepository>();
        serviceCollection.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();

        serviceCollection.AddScoped<ITransactionManager, EfTransactionManager>();
    }
}
