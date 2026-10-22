using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderService.Application.Common.Interfaces;

namespace OrderService.Infrastructure;

public class DbContextFactory : IOrderDbContextFactory
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

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        var context = new OrderDbContext(optionsBuilder.Options);

        context.Database.EnsureCreated();

        context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerEmail TEXT NOT NULL,
                OrderDate TEXT NOT NULL,
                BranchName TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS OrderBooks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                BookId INTEGER NOT NULL,
                BookTitle TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL,
                FOREIGN KEY (OrderId) REFERENCES Orders (Id) ON DELETE CASCADE
            );
        ");



        return context;
    }
}
