using Microsoft.EntityFrameworkCore;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        MiniOrderDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        var categories = new List<Category>
        {
            new()
            {
                Name = "Computers",
                Slug = "computers",
                IsActive = true,
            },
            new()
            {
                Name = "Accessories",
                Slug = "accessories",
                IsActive = true,
            },
            new()
            {
                Name = "Monitors",
                Slug = "monitors",
                IsActive = true,
            },
            new()
            {
                Name = "Audio",
                Slug = "audio",
                IsActive = true,
            },
        };

        if (!await dbContext.Categories.AnyAsync(cancellationToken))
        {
            await dbContext.Categories.AddRangeAsync(categories, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var computersCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "computers",
            cancellationToken
        );

        var accessoriesCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "accessories",
            cancellationToken
        );

        var monitorsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "monitors",
            cancellationToken
        );

        var audioCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "audio",
            cancellationToken
        );

        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var products = new List<Product>
        {
            new()
            {
                CategoryId = computersCategory.Id,
                StockCode = "ELC-LPT-001",
                Name = "Laptop",
                Description = "High-performance laptop for everyday work and productivity.",
                Price = 45000m,
                StockQuantity = 10,
                IsActive = true,
            },
            new()
            {
                CategoryId = accessoriesCategory.Id,
                StockCode = "ACC-MSE-002",
                Name = "Wireless Mouse",
                Description = "Wireless mouse designed for comfortable everyday use.",
                Price = 750m,
                StockQuantity = 100,
                IsActive = true,
            },
            new()
            {
                CategoryId = accessoriesCategory.Id,
                StockCode = "ACC-KEY-003",
                Name = "Mechanical Keyboard",
                Description = "Mechanical keyboard designed for productivity and gaming.",
                Price = 2200m,
                StockQuantity = 40,
                IsActive = true,
            },
            new()
            {
                CategoryId = monitorsCategory.Id,
                StockCode = "ELC-MON-004",
                Name = "27-inch Monitor",
                Description = "27-inch monitor suitable for work and entertainment.",
                Price = 8900m,
                StockQuantity = 20,
                IsActive = true,
            },
            new()
            {
                CategoryId = audioCategory.Id,
                StockCode = "ACC-HDS-005",
                Name = "USB Headset",
                Description = "USB headset with microphone for calls and everyday use.",
                Price = 2500m,
                StockQuantity = 35,
                IsActive = true,
            },
        };

        await dbContext.Products.AddRangeAsync(products, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
