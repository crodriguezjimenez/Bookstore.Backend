using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure;


public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
            entity.Property(b => b.Price).HasPrecision(18, 2);
        });
    }
}

