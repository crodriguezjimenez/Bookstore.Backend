using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Common.Interfaces;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddOrderInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IOrderDbContextFactory, DbContextFactory>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<IMessageBusPublisher, RabbitMqPublisher>();

        return services;
    }
}
