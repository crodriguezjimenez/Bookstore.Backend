using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderBook> OrderBook { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.CustomerEmail).IsRequired().HasMaxLength(150);
            entity.Property(o => o.BranchName).IsRequired().HasMaxLength(50);

            entity.HasMany(o => o.Items)
                  .WithOne()
                  .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<OrderBook>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.BookTitle).IsRequired().HasMaxLength(250);
            entity.Property(oi => oi.UnitPrice).HasConversion<double>(); 
        });
    }
}
