using GadgetHub.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductOrder> ProductOrders { get; set; }
    public DbSet<StoredQuotation> Quotations { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderStatus>()
            .Property(o => o.PricePerUnit)
            .HasPrecision(18, 2);

        modelBuilder.Entity<StoredQuotation>()
            .Property(q => q.PricePerUnit)
            .HasPrecision(18, 2);

        // Simplified ProductOrder configuration
        modelBuilder.Entity<ProductOrder>()
            .HasKey(po => po.Id);
    }
}