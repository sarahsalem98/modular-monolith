using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ordering");

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(o => o.UnitPrice).HasPrecision(18, 2);
            entity.Property(o => o.TotalPrice).HasPrecision(18, 2);
            entity.Property(o => o.Status).HasConversion<string>();

            // Notice: NO foreign key to Catalog or Customers tables!
            // We only store the IDs — cross-module data is fetched via contracts
            entity.HasIndex(o => o.CustomerId);
            entity.HasIndex(o => o.ProductId);
        });
    }
}
