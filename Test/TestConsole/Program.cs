using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Common.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure;
using System.Diagnostics;

namespace OrderService.TestConsole;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== PRUEBA MULTIBANCO EN PARALELO: ORDER SERVICE ===\n");

        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<IOrderDbContextFactory, DbContextFactory>()
            .BuildServiceProvider();

        var factory = serviceProvider.GetRequiredService<IOrderDbContextFactory>();
        var sucursales = new List<string> { "Norte", "Este", "Oeste" };

        Console.WriteLine("[1/2] Verificando y sembrando órdenes de prueba en las 3 bases de datos...");
        foreach (var sucursal in sucursales)
        {
            using var context = (OrderDbContext)factory.CreateDbContext(sucursal);
            if (!context.Orders.Any())
            {
                var orden = new Order
                {
                    CustomerEmail = $"cliente.{sucursal.ToLower()}@deloitte.com",
                    BranchName = sucursal,
                    OrderDate = DateTime.UtcNow,
                    Items = new List<OrderBook>
                    {
                        new OrderBook { BookId = 1, BookTitle = $"Libro Arquitectura - Sucursal {sucursal}", Quantity = 1, UnitPrice = 500.00m },
                        new OrderBook { BookId = 2, BookTitle = $"Libro Microservicios - Sucursal {sucursal}", Quantity = 2, UnitPrice = 350.00m }
                    }
                };
                context.Orders.Add(orden);
                context.SaveChanges();
                Console.WriteLine($"  [✔] Orden sembrada en Sucursal{sucursal}DB.");
            }
            else
            {
                Console.WriteLine($"  [i] Sucursal{sucursal}DB ya contiene datos.");
            }
        }

        Console.WriteLine("\n[2/2] Consultando las 3 sucursales simultáneamente (Task.WhenAll)...");
        var cronometro = Stopwatch.StartNew();

        
        var tareasConsulta = sucursales.Select(sucursal => ConsultarOrdenesPorSucursalAsync(factory, sucursal)).ToList();

        
        var resultadosPorSucursal = await Task.WhenAll(tareasConsulta);
        cronometro.Stop();

        
        var todasLasOrdenes = resultadosPorSucursal.SelectMany(r => r).ToList();

        
        Console.WriteLine($"\n================ RESULTADOS CONSOLIDADOS ================");
        Console.WriteLine($"Tiempo total de respuesta: {cronometro.ElapsedMilliseconds} ms");
        Console.WriteLine($"Total de órdenes recuperadas: {todasLasOrdenes.Count}\n");

        foreach (var orden in todasLasOrdenes)
        {
            Console.WriteLine($"-> [SUCURSAL: {orden.BranchName.ToUpper()}] | Orden #{orden.Id} | Cliente: {orden.CustomerEmail}");
            var totalAmount = orden.Items.Sum(item => item.Quantity * item.UnitPrice);
            foreach (var item in orden.Items)
            {
                Console.WriteLine($"     - {item.BookTitle} (Cant: {item.Quantity}) - ${item.UnitPrice:N2} c/u");
            }
            Console.WriteLine(new string('-', 55));
            Console.WriteLine($"   Total: ${totalAmount:N2} | Fecha: {orden.OrderDate:yyyy-MM-dd HH:mm:ss}");
            
        }

        Console.WriteLine("\n=== PRUEBA FINALIZADA CON ÉXITO ===");
    }

    
    private static async Task<List<Order>> ConsultarOrdenesPorSucursalAsync(IOrderDbContextFactory factory, string branchName)
    {
        using var context = (OrderDbContext)factory.CreateDbContext(branchName);
        return await context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync();
    }
}
