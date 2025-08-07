using GadgetHub.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ProductOrder> ProductOrders { get; set; }
    public DbSet<StoredQuotation> Quotations { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure decimal precision
        modelBuilder.Entity<OrderStatus>()
            .Property(o => o.PricePerUnit)
            .HasPrecision(18, 2);

        modelBuilder.Entity<StoredQuotation>()
            .Property(q => q.PricePerUnit)
            .HasPrecision(18, 2);

        // Configure relationships
        modelBuilder.Entity<ProductOrder>()
            .HasOne(po => po.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(po => po.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore the Customer class if you're not using it
        modelBuilder.Ignore<Customer>();
    }
}