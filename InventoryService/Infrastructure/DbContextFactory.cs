using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using InventoryService.Domain;
using InventoryService.Application.Common.Interfaces;

namespace InventoryService.Infrastructure;

public class DbContextFactory : IInventoryDbContextFactory
{
    private readonly IConfiguration _configuration;

    public DbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbContext CreateDbContext(string branchName)
    {
        var connectionString = _configuration.GetConnectionString($"Sucursal{branchName}DB");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException($"No se encontró la cadena de conexión para la sucursal: {branchName}");
        }

        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        var context = new InventoryDbContext(optionsBuilder.Options);

        context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                Price REAL NOT NULL,
                Stock INTEGER NOT NULL
            );
        ");

        //context.Database.EnsureCreated();
        SeedInitialData(context, branchName);

        return context;
    }


    private static void SeedInitialData(InventoryDbContext context, string branchName)
    {
        if (!context.Books.Any())
        {
            context.Books.AddRange(
                new Book
                {
                    Title = $"Clean Architecture en .NET - Edición {branchName}",
                    Author = "Robert C. Martin",
                    Price = 550.00m,
                    Stock = 12
                },
                new Book
                {
                    Title = $"Microservicios Prácticos - {branchName}",
                    Author = "Sam Newman",
                    Price = 480.00m,
                    Stock = 8
                }
            );
            context.SaveChanges();
        }
    }
}
