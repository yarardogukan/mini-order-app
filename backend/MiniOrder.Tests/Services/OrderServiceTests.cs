using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiniOrder.Application.DTOs.Orders.Requests;
using MiniOrder.Application.Validators.Orders;
using MiniOrder.Infrastructure.Persistence;
using MiniOrder.Infrastructure.Services;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Tests.Services;

public sealed class OrderServiceTests
{

    [Fact]
    public async Task CreateAsync_WhenStockIsInsufficient_ShouldNotCreateOrderOrChangeStock()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            dbContext.Products.AddRange(
                new Product
                {
                    StockCode = "TEST-001",
                    Name = "Test Product 1",
                    Price = 1000m,
                    StockQuantity = 10
                },
                new Product
                {
                    StockCode = "TEST-002",
                    Name = "Test Product 2",
                    Price = 500m,
                    StockQuantity = 5
                });

            await dbContext.SaveChangesAsync();

            var service = CreateOrderService(dbContext);

            var request = new CreateOrderRequest(
                "Doğukan",
                [
                    new CreateOrderItemRequest(1, 2),
                new CreateOrderItemRequest(2, 10)
                ]);

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(
                "Product.InsufficientStock",
                result.Error.Code);

            Assert.Empty(
                await dbContext.Orders.ToListAsync());

            var firstProduct = await dbContext.Products
                .SingleAsync(product => product.Id == 1);

            var secondProduct = await dbContext.Products
                .SingleAsync(product => product.Id == 2);

            Assert.Equal(10, firstProduct.StockQuantity);
            Assert.Equal(5, secondProduct.StockQuantity);
        }
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldCreateOrderAndDecreaseStock()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            dbContext.Products.AddRange(
                new Product
                {
                    StockCode = "TEST-001",
                    Name = "Test Product 1",
                    Price = 1000m,
                    StockQuantity = 10
                },
                new Product
                {
                    StockCode = "TEST-002",
                    Name = "Test Product 2",
                    Price = 500m,
                    StockQuantity = 5
                });

            await dbContext.SaveChangesAsync();

            var service = CreateOrderService(dbContext);

            var request = new CreateOrderRequest(
                "Doğukan",
                [
                    new CreateOrderItemRequest(1, 2),
                new CreateOrderItemRequest(2, 3)
                ]);

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal(3500m, result.Value.TotalAmount);
            Assert.Equal(2, result.Value.Items.Count);

            var order = await dbContext.Orders
                .Include(order => order.Items)
                .SingleAsync();

            Assert.Equal("Doğukan", order.CustomerName);
            Assert.Equal(3500m, order.TotalAmount);
            Assert.Equal(2, order.Items.Count);

            var firstProduct = await dbContext.Products
                .SingleAsync(product => product.Id == 1);

            var secondProduct = await dbContext.Products
                .SingleAsync(product => product.Id == 2);

            Assert.Equal(8, firstProduct.StockQuantity);
            Assert.Equal(2, secondProduct.StockQuantity);
        }
    }


    #region Helpers

    private static async Task<(SqliteConnection Connection, MiniOrderDbContext DbContext)>
        CreateDbContextAsync()
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

    private static OrderService CreateOrderService(
        MiniOrderDbContext dbContext)
    {
        IValidator<CreateOrderItemRequest> itemValidator =
            new CreateOrderItemRequestValidator();

        IValidator<CreateOrderRequest> orderValidator =
            new CreateOrderRequestValidator(itemValidator);

        return new OrderService(
            dbContext,
            NullLogger<OrderService>.Instance,
            orderValidator);
    }

    #endregion
}