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

        await SeedCategoriesAsync(dbContext, cancellationToken);
        await SeedBrandsAsync(dbContext, cancellationToken);
        await SeedProductsAsync(dbContext, cancellationToken);
        await SeedCategoryAttributesAsync(dbContext, cancellationToken);
        await SeedProductAttributeValuesAsync(dbContext, cancellationToken);
    }

    private static async Task SeedCategoriesAsync(
        MiniOrderDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var rootCategories = new List<Category>
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

        foreach (var category in rootCategories)
        {
            var exists = await dbContext.Categories.AnyAsync(
                existingCategory => existingCategory.Slug == category.Slug,
                cancellationToken
            );

            if (!exists)
            {
                await dbContext.Categories.AddAsync(category, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

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

        var subCategories = new List<Category>
        {
            new()
            {
                Name = "Laptops",
                Slug = "laptops",
                IsActive = true,
                ParentCategoryId = computersCategory.Id,
            },
            new()
            {
                Name = "Mouse",
                Slug = "mouse",
                IsActive = true,
                ParentCategoryId = accessoriesCategory.Id,
            },
            new()
            {
                Name = "Keyboards",
                Slug = "keyboards",
                IsActive = true,
                ParentCategoryId = accessoriesCategory.Id,
            },
            new()
            {
                Name = "Computer Monitors",
                Slug = "computer-monitors",
                IsActive = true,
                ParentCategoryId = monitorsCategory.Id,
            },
            new()
            {
                Name = "Headsets",
                Slug = "headsets",
                IsActive = true,
                ParentCategoryId = audioCategory.Id,
            },
        };

        foreach (var category in subCategories)
        {
            var exists = await dbContext.Categories.AnyAsync(
                existingCategory => existingCategory.Slug == category.Slug,
                cancellationToken
            );

            if (!exists)
            {
                await dbContext.Categories.AddAsync(category, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedBrandsAsync(
        MiniOrderDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var brands = new List<Brand>
        {
            new()
            {
                Name = "Dell",
                Slug = "dell",
                IsActive = true,
            },
            new()
            {
                Name = "Logitech",
                Slug = "logitech",
                IsActive = true,
            },
            new()
            {
                Name = "Keychron",
                Slug = "keychron",
                IsActive = true,
            },
            new()
            {
                Name = "Samsung",
                Slug = "samsung",
                IsActive = true,
            },
            new()
            {
                Name = "Jabra",
                Slug = "jabra",
                IsActive = true,
            },
        };

        foreach (var brand in brands)
        {
            var exists = await dbContext.Brands.AnyAsync(
                existingBrand => existingBrand.Slug == brand.Slug,
                cancellationToken
            );

            if (!exists)
            {
                await dbContext.Brands.AddAsync(brand, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedProductsAsync(
        MiniOrderDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var laptopsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "laptops",
            cancellationToken
        );

        var mouseCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "mouse",
            cancellationToken
        );

        var keyboardsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "keyboards",
            cancellationToken
        );

        var computerMonitorsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "computer-monitors",
            cancellationToken
        );

        var headsetsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "headsets",
            cancellationToken
        );

        var dellBrand = await dbContext.Brands.SingleAsync(
            brand => brand.Slug == "dell",
            cancellationToken
        );

        var logitechBrand = await dbContext.Brands.SingleAsync(
            brand => brand.Slug == "logitech",
            cancellationToken
        );

        var keychronBrand = await dbContext.Brands.SingleAsync(
            brand => brand.Slug == "keychron",
            cancellationToken
        );

        var samsungBrand = await dbContext.Brands.SingleAsync(
            brand => brand.Slug == "samsung",
            cancellationToken
        );

        var jabraBrand = await dbContext.Brands.SingleAsync(
            brand => brand.Slug == "jabra",
            cancellationToken
        );

        var products = new List<Product>
        {
            new()
            {
                CategoryId = laptopsCategory.Id,
                BrandId = dellBrand.Id,
                StockCode = "ELC-LPT-001",
                Name = "Laptop",
                Description = "High-performance laptop for everyday work and productivity.",
                Price = 45000m,
                StockQuantity = 10,
                IsActive = true,
            },
            new()
            {
                CategoryId = mouseCategory.Id,
                BrandId = logitechBrand.Id,
                StockCode = "ACC-MSE-002",
                Name = "Wireless Mouse",
                Description = "Wireless mouse designed for comfortable everyday use.",
                Price = 750m,
                StockQuantity = 100,
                IsActive = true,
            },
            new()
            {
                CategoryId = keyboardsCategory.Id,
                BrandId = keychronBrand.Id,
                StockCode = "ACC-KEY-003",
                Name = "Mechanical Keyboard",
                Description = "Mechanical keyboard designed for productivity and gaming.",
                Price = 2200m,
                StockQuantity = 40,
                IsActive = true,
            },
            new()
            {
                CategoryId = computerMonitorsCategory.Id,
                BrandId = samsungBrand.Id,
                StockCode = "ELC-MON-004",
                Name = "27-inch Monitor",
                Description = "27-inch monitor suitable for work and entertainment.",
                Price = 8900m,
                StockQuantity = 20,
                IsActive = true,
            },
            new()
            {
                CategoryId = headsetsCategory.Id,
                BrandId = jabraBrand.Id,
                StockCode = "ACC-HDS-005",
                Name = "USB Headset",
                Description = "USB headset with microphone for calls and everyday use.",
                Price = 2500m,
                StockQuantity = 35,
                IsActive = true,
            },
        };

        foreach (var product in products)
        {
            var exists = await dbContext.Products.AnyAsync(
                existingProduct => existingProduct.StockCode == product.StockCode,
                cancellationToken
            );

            if (!exists)
            {
                await dbContext.Products.AddAsync(product, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedCategoryAttributesAsync(
        MiniOrderDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var laptopsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "laptops",
            cancellationToken
        );

        var mouseCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "mouse",
            cancellationToken
        );

        var keyboardsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "keyboards",
            cancellationToken
        );

        var computerMonitorsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "computer-monitors",
            cancellationToken
        );

        var headsetsCategory = await dbContext.Categories.SingleAsync(
            category => category.Slug == "headsets",
            cancellationToken
        );

        var attributes = new List<CategoryAttribute>
        {
            // Laptops
            new()
            {
                CategoryId = laptopsCategory.Id,
                Name = "Processor",
                Code = "processor",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 1,
            },
            new()
            {
                CategoryId = laptopsCategory.Id,
                Name = "Memory",
                Code = "memory",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 2,
            },
            new()
            {
                CategoryId = laptopsCategory.Id,
                Name = "Storage",
                Code = "storage",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 3,
            },
            new()
            {
                CategoryId = laptopsCategory.Id,
                Name = "Screen Size",
                Code = "screen-size",
                DataType = "TEXT",
                IsRequired = false,
                SortOrder = 4,
            },
            // Mouse
            new()
            {
                CategoryId = mouseCategory.Id,
                Name = "DPI",
                Code = "dpi",
                DataType = "NUMBER",
                IsRequired = true,
                SortOrder = 1,
            },
            new()
            {
                CategoryId = mouseCategory.Id,
                Name = "Connection Type",
                Code = "connection-type",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 2,
            },
            new()
            {
                CategoryId = mouseCategory.Id,
                Name = "Sensor",
                Code = "sensor",
                DataType = "TEXT",
                IsRequired = false,
                SortOrder = 3,
            },
            new()
            {
                CategoryId = mouseCategory.Id,
                Name = "Weight",
                Code = "weight",
                DataType = "TEXT",
                IsRequired = false,
                SortOrder = 4,
            },
            // Keyboards
            new()
            {
                CategoryId = keyboardsCategory.Id,
                Name = "Switch Type",
                Code = "switch-type",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 1,
            },
            new()
            {
                CategoryId = keyboardsCategory.Id,
                Name = "Layout",
                Code = "layout",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 2,
            },
            new()
            {
                CategoryId = keyboardsCategory.Id,
                Name = "Connection Type",
                Code = "connection-type",
                DataType = "TEXT",
                IsRequired = false,
                SortOrder = 3,
            },
            // Monitors
            new()
            {
                CategoryId = computerMonitorsCategory.Id,
                Name = "Screen Size",
                Code = "screen-size",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 1,
            },
            new()
            {
                CategoryId = computerMonitorsCategory.Id,
                Name = "Resolution",
                Code = "resolution",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 2,
            },
            new()
            {
                CategoryId = computerMonitorsCategory.Id,
                Name = "Refresh Rate",
                Code = "refresh-rate",
                DataType = "TEXT",
                IsRequired = false,
                SortOrder = 3,
            },
            // Headsets
            new()
            {
                CategoryId = headsetsCategory.Id,
                Name = "Connection Type",
                Code = "connection-type",
                DataType = "TEXT",
                IsRequired = true,
                SortOrder = 1,
            },
            new()
            {
                CategoryId = headsetsCategory.Id,
                Name = "Microphone",
                Code = "microphone",
                DataType = "BOOLEAN",
                IsRequired = true,
                SortOrder = 2,
            },
            new()
            {
                CategoryId = headsetsCategory.Id,
                Name = "Frequency Response",
                Code = "frequency-response",
                DataType = "TEXT",
                IsRequired = false,
                SortOrder = 3,
            },
        };

        foreach (var attribute in attributes)
        {
            var exists = await dbContext.CategoryAttributes.AnyAsync(
                existingAttribute =>
                    existingAttribute.CategoryId == attribute.CategoryId
                    && existingAttribute.Code == attribute.Code,
                cancellationToken
            );

            if (!exists)
            {
                await dbContext.CategoryAttributes.AddAsync(attribute, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedProductAttributeValuesAsync(
        MiniOrderDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var products = await dbContext.Products.ToDictionaryAsync(
            product => product.StockCode,
            cancellationToken
        );

        var attributes = await dbContext.CategoryAttributes.ToListAsync(cancellationToken);

        CategoryAttribute GetAttribute(int categoryId, string code)
        {
            return attributes.Single(attribute =>
                attribute.CategoryId == categoryId && attribute.Code == code
            );
        }

        var laptop = products["ELC-LPT-001"];
        var mouse = products["ACC-MSE-002"];
        var keyboard = products["ACC-KEY-003"];
        var monitor = products["ELC-MON-004"];
        var headset = products["ACC-HDS-005"];

        var values = new List<ProductAttributeValue>
        {
            // Laptop
            new()
            {
                ProductId = laptop.Id,
                CategoryAttributeId = GetAttribute(laptop.CategoryId, "processor").Id,
                Value = "Intel Core i7",
            },
            new()
            {
                ProductId = laptop.Id,
                CategoryAttributeId = GetAttribute(laptop.CategoryId, "memory").Id,
                Value = "16 GB DDR5",
            },
            new()
            {
                ProductId = laptop.Id,
                CategoryAttributeId = GetAttribute(laptop.CategoryId, "storage").Id,
                Value = "512 GB SSD",
            },
            new()
            {
                ProductId = laptop.Id,
                CategoryAttributeId = GetAttribute(laptop.CategoryId, "screen-size").Id,
                Value = "15.6 inch",
            },
            // Mouse
            new()
            {
                ProductId = mouse.Id,
                CategoryAttributeId = GetAttribute(mouse.CategoryId, "dpi").Id,
                Value = "16000",
            },
            new()
            {
                ProductId = mouse.Id,
                CategoryAttributeId = GetAttribute(mouse.CategoryId, "connection-type").Id,
                Value = "Wireless",
            },
            new()
            {
                ProductId = mouse.Id,
                CategoryAttributeId = GetAttribute(mouse.CategoryId, "sensor").Id,
                Value = "Optical",
            },
            new()
            {
                ProductId = mouse.Id,
                CategoryAttributeId = GetAttribute(mouse.CategoryId, "weight").Id,
                Value = "89 g",
            },
            // Keyboard
            new()
            {
                ProductId = keyboard.Id,
                CategoryAttributeId = GetAttribute(keyboard.CategoryId, "switch-type").Id,
                Value = "Mechanical",
            },
            new()
            {
                ProductId = keyboard.Id,
                CategoryAttributeId = GetAttribute(keyboard.CategoryId, "layout").Id,
                Value = "US QWERTY",
            },
            new()
            {
                ProductId = keyboard.Id,
                CategoryAttributeId = GetAttribute(keyboard.CategoryId, "connection-type").Id,
                Value = "USB-C",
            },
            // Monitor
            new()
            {
                ProductId = monitor.Id,
                CategoryAttributeId = GetAttribute(monitor.CategoryId, "screen-size").Id,
                Value = "27 inch",
            },
            new()
            {
                ProductId = monitor.Id,
                CategoryAttributeId = GetAttribute(monitor.CategoryId, "resolution").Id,
                Value = "2560 x 1440",
            },
            new()
            {
                ProductId = monitor.Id,
                CategoryAttributeId = GetAttribute(monitor.CategoryId, "refresh-rate").Id,
                Value = "144 Hz",
            },
            // Headset
            new()
            {
                ProductId = headset.Id,
                CategoryAttributeId = GetAttribute(headset.CategoryId, "connection-type").Id,
                Value = "USB",
            },
            new()
            {
                ProductId = headset.Id,
                CategoryAttributeId = GetAttribute(headset.CategoryId, "microphone").Id,
                Value = "true",
            },
            new()
            {
                ProductId = headset.Id,
                CategoryAttributeId = GetAttribute(headset.CategoryId, "frequency-response").Id,
                Value = "20 Hz - 20 kHz",
            },
        };

        foreach (var value in values)
        {
            var exists = await dbContext.ProductAttributeValues.AnyAsync(
                existingValue =>
                    existingValue.ProductId == value.ProductId
                    && existingValue.CategoryAttributeId == value.CategoryAttributeId,
                cancellationToken
            );

            if (!exists)
            {
                await dbContext.ProductAttributeValues.AddAsync(value, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
