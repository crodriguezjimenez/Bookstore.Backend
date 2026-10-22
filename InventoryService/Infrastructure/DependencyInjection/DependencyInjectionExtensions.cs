using InventoryService.Application.Common.Interfaces;
using InventoryService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Infrastructure.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryDbContextFactory, DbContextFactory>();
        services.AddScoped<IBookRepository, BookRepository>();
        return services; 
    }
}


