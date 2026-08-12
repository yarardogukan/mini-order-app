using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiniOrder.Application.DTOs.Carts.Requests;
using MiniOrder.Application.Validators.Carts;
using MiniOrder.Domain.Entities;
using MiniOrder.Infrastructure.Persistence;
using MiniOrder.Infrastructure.Services;

namespace MiniOrder.Tests.Services;

public sealed class CartServiceTests
{
    [Fact]
    public async Task AddItemAsync_WhenCartDoesNotExist_ShouldCreateCartAndAddItem()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();
        var product = await SeedProductAsync(dbContext);

        await using (connection)
        await using (dbContext)
        {
            var service = CreateCartService(dbContext);

            var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 2 };

            // Act
            var result = await service.AddItemAsync(null, request);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.NotEqual(Guid.Empty, result.Value.CartId);
            Assert.Equal(2, result.Value.ItemCount);
            Assert.Equal(2000m, result.Value.Subtotal);
            Assert.Equal(2000m, result.Value.Total);

            var item = Assert.Single(result.Value.Items);

            Assert.Equal(product.Id, item.ProductId);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(1000m, item.UnitPrice);
            Assert.Equal(2000m, item.LineTotal);

            var storedCart = await dbContext.Carts.Include(cart => cart.Items).SingleAsync();

            Assert.Single(storedCart.Items);
            Assert.Equal(2, storedCart.Items.Single().Quantity);

            var storedProduct = await dbContext.Products.SingleAsync(existingProduct =>
                existingProduct.Id == product.Id
            );

            Assert.Equal(10, storedProduct.StockQuantity);
        }
    }

    [Fact]
    public async Task AddItemAsync_WhenProductAlreadyExists_ShouldIncreaseQuantityWithoutCreatingDuplicate()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();
        var product = await SeedProductAsync(dbContext);

        await using (connection)
        await using (dbContext)
        {
            var service = CreateCartService(dbContext);

            var firstRequest = new AddCartItemRequest { ProductId = product.Id, Quantity = 2 };

            var firstResult = await service.AddItemAsync(null, firstRequest);

            Assert.True(firstResult.IsSuccess);

            var secondRequest = new AddCartItemRequest { ProductId = product.Id, Quantity = 3 };

            // Act
            var result = await service.AddItemAsync(firstResult.Value.CartId, secondRequest);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal(5, result.Value.ItemCount);
            Assert.Equal(5000m, result.Value.Subtotal);

            var item = Assert.Single(result.Value.Items);

            Assert.Equal(5, item.Quantity);

            var storedItems = await dbContext
                .CartItems.Where(cartItem => cartItem.CartId == result.Value.CartId)
                .ToListAsync();

            Assert.Single(storedItems);
            Assert.Equal(5, storedItems[0].Quantity);

            var storedProduct = await dbContext.Products.SingleAsync(existingProduct =>
                existingProduct.Id == product.Id
            );

            Assert.Equal(10, storedProduct.StockQuantity);
        }
    }

    [Fact]
    public async Task AddItemAsync_WhenRequestedQuantityExceedsStock_ShouldFailAndNotChangeStock()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();
        var product = await SeedProductAsync(dbContext);

        await using (connection)
        await using (dbContext)
        {
            var service = CreateCartService(dbContext);

            var request = new AddCartItemRequest { ProductId = product.Id, Quantity = 11 };

            // Act
            var result = await service.AddItemAsync(null, request);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Cart.InsufficientStock", result.Error.Code);

            Assert.Empty(await dbContext.CartItems.ToListAsync());

            var storedProduct = await dbContext.Products.SingleAsync(existingProduct =>
                existingProduct.Id == product.Id
            );

            Assert.Equal(10, storedProduct.StockQuantity);
        }
    }

    [Fact]
    public async Task GetCartAsync_WhenCartHasItems_ShouldCalculateTotalsCorrectly()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();
        var (category, brand) = await SeedCatalogDependenciesAsync(dbContext);

        await using (connection)
        await using (dbContext)
        {
            var firstProduct = new Product
            {
                CategoryId = category.Id,
                BrandId = brand.Id,
                StockCode = "TEST-001",
                Name = "Test Product 1",
                Description = "First test product.",
                Price = 1000m,
                StockQuantity = 10,
                IsActive = true,
            };

            var secondProduct = new Product
            {
                CategoryId = category.Id,
                BrandId = brand.Id,
                StockCode = "TEST-002",
                Name = "Test Product 2",
                Description = "Second test product.",
                Price = 500m,
                StockQuantity = 10,
                IsActive = true,
            };

            dbContext.Products.AddRange(firstProduct, secondProduct);

            await dbContext.SaveChangesAsync();

            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            cart.Items.Add(new CartItem { ProductId = firstProduct.Id, Quantity = 2 });

            cart.Items.Add(new CartItem { ProductId = secondProduct.Id, Quantity = 3 });

            dbContext.Carts.Add(cart);

            await dbContext.SaveChangesAsync();

            var service = CreateCartService(dbContext);

            // Act
            var result = await service.GetCartAsync(cart.Id);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal(cart.Id, result.Value.CartId);

            // 2 + 3
            Assert.Equal(5, result.Value.ItemCount);

            // (1000 * 2) + (500 * 3)
            Assert.Equal(3500m, result.Value.Subtotal);
            Assert.Equal(3500m, result.Value.Total);

            Assert.Equal(2, result.Value.Items.Count);

            var firstItem = result.Value.Items.Single(item => item.ProductId == firstProduct.Id);

            Assert.Equal(1000m, firstItem.UnitPrice);
            Assert.Equal(2, firstItem.Quantity);
            Assert.Equal(2000m, firstItem.LineTotal);

            var secondItem = result.Value.Items.Single(item => item.ProductId == secondProduct.Id);

            Assert.Equal(500m, secondItem.UnitPrice);
            Assert.Equal(3, secondItem.Quantity);
            Assert.Equal(1500m, secondItem.LineTotal);
        }
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_ShouldUpdateQuantityAndRejectStockOverflow()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();
        var product = await SeedProductAsync(dbContext);

        await using (connection)
        await using (dbContext)
        {
            var cart = await SeedCartAsync(dbContext, product, 2);

            var service = CreateCartService(dbContext);

            var validRequest = new UpdateCartItemRequest { Quantity = 5 };

            // Act - valid update
            var successResult = await service.UpdateItemQuantityAsync(
                cart.Id,
                product.Id,
                validRequest
            );

            // Assert - valid update
            Assert.True(successResult.IsSuccess);

            var updatedItem = Assert.Single(successResult.Value.Items);

            Assert.Equal(5, updatedItem.Quantity);
            Assert.Equal(5000m, updatedItem.LineTotal);

            var invalidRequest = new UpdateCartItemRequest { Quantity = 11 };

            // Act - stock overflow
            var failureResult = await service.UpdateItemQuantityAsync(
                cart.Id,
                product.Id,
                invalidRequest
            );

            // Assert - stock overflow
            Assert.True(failureResult.IsFailure);
            Assert.Equal("Cart.InsufficientStock", failureResult.Error.Code);

            var storedItem = await dbContext.CartItems.SingleAsync(cartItem =>
                cartItem.CartId == cart.Id && cartItem.ProductId == product.Id
            );

            Assert.Equal(5, storedItem.Quantity);

            var storedProduct = await dbContext.Products.SingleAsync(existingProduct =>
                existingProduct.Id == product.Id
            );

            Assert.Equal(10, storedProduct.StockQuantity);
        }
    }

    [Fact]
    public async Task RemoveItemAsync_WhenItemExists_ShouldRemoveItemWithoutChangingProductStock()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();
        var product = await SeedProductAsync(dbContext);

        await using (connection)
        await using (dbContext)
        {
            var cart = await SeedCartAsync(dbContext, product, 3);

            var service = CreateCartService(dbContext);

            // Act
            var result = await service.RemoveItemAsync(cart.Id, product.Id);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal(0, result.Value.ItemCount);
            Assert.Equal(0m, result.Value.Subtotal);
            Assert.Equal(0m, result.Value.Total);
            Assert.Empty(result.Value.Items);

            Assert.Empty(
                await dbContext
                    .CartItems.Where(cartItem => cartItem.CartId == cart.Id)
                    .ToListAsync()
            );

            var storedProduct = await dbContext.Products.SingleAsync(existingProduct =>
                existingProduct.Id == product.Id
            );

            Assert.Equal(10, storedProduct.StockQuantity);
        }
    }

    [Fact]
    public async Task ClearAsync_WhenCartHasItems_ShouldClearCartAndRemainIdempotent()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();
        var product = await SeedProductAsync(dbContext);

        await using (connection)
        await using (dbContext)
        {
            var cart = await SeedCartAsync(dbContext, product, 2);

            var service = CreateCartService(dbContext);

            // Act
            var firstResult = await service.ClearAsync(cart.Id);

            // Assert
            Assert.True(firstResult.IsSuccess);

            Assert.Empty(
                await dbContext
                    .CartItems.Where(cartItem => cartItem.CartId == cart.Id)
                    .ToListAsync()
            );

            // Act again - already empty
            var secondResult = await service.ClearAsync(cart.Id);

            // Assert idempotency
            Assert.True(secondResult.IsSuccess);

            var storedProduct = await dbContext.Products.SingleAsync(existingProduct =>
                existingProduct.Id == product.Id
            );

            Assert.Equal(10, storedProduct.StockQuantity);
        }
    }

    #region Helpers

    private static async Task<(
        SqliteConnection Connection,
        MiniOrderDbContext DbContext
    )> CreateDbContextAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MiniOrderDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new MiniOrderDbContext(options);

        await dbContext.Database.EnsureCreatedAsync();

        return (connection, dbContext);
    }

    private static CartService CreateCartService(MiniOrderDbContext dbContext)
    {
        IValidator<AddCartItemRequest> addItemValidator = new AddCartItemRequestValidator();

        IValidator<UpdateCartItemRequest> updateItemValidator =
            new UpdateCartItemRequestValidator();

        return new CartService(
            dbContext,
            NullLogger<CartService>.Instance,
            addItemValidator,
            updateItemValidator
        );
    }

    private static async Task<Product> SeedProductAsync(MiniOrderDbContext dbContext)
    {
        var (category, brand) = await SeedCatalogDependenciesAsync(dbContext);

        var product = new Product
        {
            CategoryId = category.Id,
            BrandId = brand.Id,
            StockCode = "TEST-001",
            Name = "Test Product",
            Description = "Test product description.",
            Price = 1000m,
            StockQuantity = 10,
            IsActive = true,
        };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        return product;
    }

    private static async Task<Cart> SeedCartAsync(
        MiniOrderDbContext dbContext,
        Product product,
        int quantity
    )
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        cart.Items.Add(new CartItem { ProductId = product.Id, Quantity = quantity });

        dbContext.Carts.Add(cart);

        await dbContext.SaveChangesAsync();

        return cart;
    }

    private static async Task<(Category Category, Brand Brand)> SeedCatalogDependenciesAsync(
        MiniOrderDbContext dbContext
    )
    {
        var category = new Category
        {
            Name = $"Test Category {Guid.NewGuid():N}",
            Slug = $"test-category-{Guid.NewGuid():N}",
            IsActive = true,
        };

        var brand = new Brand
        {
            Name = $"Test Brand {Guid.NewGuid():N}",
            Slug = $"test-brand-{Guid.NewGuid():N}",
            IsActive = true,
        };

        dbContext.Categories.Add(category);
        dbContext.Brands.Add(brand);

        await dbContext.SaveChangesAsync();

        return (category, brand);
    }

    #endregion
}
