using Microsoft.EntityFrameworkCore;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        MiniOrderDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var products = new List<Product>
        {
            new()
            {
                StockCode = "ELC-LPT-001",
                Name = "Laptop",
                Price = 45000m,
                StockQuantity = 10
            },
            new()
            {
                StockCode = "ACC-MSE-002",
                Name = "Wireless Mouse",
                Price = 750m,
                StockQuantity = 100
            },
            new()
            {
                StockCode = "ACC-KEY-003",
                Name = "Mechanical Keyboard",
                Price = 2200m,
                StockQuantity = 40
            },
            new()
            {
                StockCode = "ELC-MON-004",
                Name = "27-inch Monitor",
                Price = 8900m,
                StockQuantity = 20
            },
            new()
            {
                StockCode = "ACC-HDS-005",
                Name = "USB Headset",
                Price = 2500m,
                StockQuantity = 35
            }
        };

        await dbContext.Products.AddRangeAsync(products, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}