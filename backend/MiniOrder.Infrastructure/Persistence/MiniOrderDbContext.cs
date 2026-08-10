using Microsoft.EntityFrameworkCore;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence;

public class MiniOrderDbContext : DbContext
{
    public MiniOrderDbContext(DbContextOptions<MiniOrderDbContext> options)
        : base(options) { }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<CategoryAttribute> CategoryAttributes => Set<CategoryAttribute>();

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiniOrderDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
