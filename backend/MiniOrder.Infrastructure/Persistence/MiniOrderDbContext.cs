using Microsoft.EntityFrameworkCore;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence;

public class MiniOrderDbContext : DbContext
{
    public MiniOrderDbContext(DbContextOptions<MiniOrderDbContext> options)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiniOrderDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
