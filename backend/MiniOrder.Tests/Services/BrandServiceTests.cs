using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiniOrder.Application.DTOs.Brands.Requests;
using MiniOrder.Application.Validators.Brands;
using MiniOrder.Domain.Entities;
using MiniOrder.Infrastructure.Persistence;
using MiniOrder.Infrastructure.Services;

namespace MiniOrder.Tests.Services;

public sealed class BrandServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldCreateBrand()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            var service = CreateBrandService(dbContext);

            var request = new CreateBrandRequest("Apple", "apple");

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal("Apple", result.Value.Name);
            Assert.Equal("apple", result.Value.Slug);
            Assert.True(result.Value.IsActive);

            var brand = await dbContext.Brands.SingleAsync();

            Assert.Equal("Apple", brand.Name);
            Assert.Equal("apple", brand.Slug);
            Assert.True(brand.IsActive);
        }
    }

    [Fact]
    public async Task CreateAsync_WhenSlugAlreadyExists_ShouldFailAndNotCreateBrand()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            dbContext.Brands.Add(
                new Brand
                {
                    Name = "Dell",
                    Slug = "dell",
                    IsActive = true,
                }
            );

            await dbContext.SaveChangesAsync();

            var service = CreateBrandService(dbContext);

            var request = new CreateBrandRequest("Another Dell", "dell");

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Brand.SlugAlreadyExists", result.Error.Code);

            var brands = await dbContext.Brands.ToListAsync();

            Assert.Single(brands);
            Assert.Equal("Dell", brands[0].Name);
        }
    }

    [Fact]
    public async Task UpdateAsync_WhenRequestIsValid_ShouldUpdateBrand()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            var brand = new Brand
            {
                Name = "Old Brand",
                Slug = "old-brand",
                IsActive = true,
            };

            dbContext.Brands.Add(brand);
            await dbContext.SaveChangesAsync();

            var service = CreateBrandService(dbContext);

            var request = new UpdateBrandRequest("New Brand", "new-brand", true);

            // Act
            var result = await service.UpdateAsync(brand.Id, request);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal("New Brand", result.Value.Name);
            Assert.Equal("new-brand", result.Value.Slug);
            Assert.True(result.Value.IsActive);

            var updatedBrand = await dbContext.Brands.SingleAsync(existingBrand =>
                existingBrand.Id == brand.Id
            );

            Assert.Equal("New Brand", updatedBrand.Name);
            Assert.Equal("new-brand", updatedBrand.Slug);
            Assert.True(updatedBrand.IsActive);
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenBrandHasActiveProducts_ShouldFail()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            var category = new Category
            {
                Name = "Laptops",
                Slug = "laptops",
                IsActive = true,
            };

            var brand = new Brand
            {
                Name = "Dell",
                Slug = "dell",
                IsActive = true,
            };

            dbContext.Categories.Add(category);
            dbContext.Brands.Add(brand);

            await dbContext.SaveChangesAsync();

            dbContext.Products.Add(
                new Product
                {
                    CategoryId = category.Id,
                    BrandId = brand.Id,
                    StockCode = "TEST-LPT-001",
                    Name = "Test Laptop",
                    Description = "Test laptop description.",
                    Price = 1000m,
                    StockQuantity = 10,
                    IsActive = true,
                }
            );

            await dbContext.SaveChangesAsync();

            var service = CreateBrandService(dbContext);

            // Act
            var result = await service.DeleteAsync(brand.Id);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Brand.HasActiveProducts", result.Error.Code);

            var storedBrand = await dbContext.Brands.SingleAsync(existingBrand =>
                existingBrand.Id == brand.Id
            );

            Assert.True(storedBrand.IsActive);
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

    private static BrandService CreateBrandService(MiniOrderDbContext dbContext)
    {
        IValidator<CreateBrandRequest> createValidator = new CreateBrandRequestValidator();

        IValidator<UpdateBrandRequest> updateValidator = new UpdateBrandRequestValidator();

        return new BrandService(
            dbContext,
            NullLogger<BrandService>.Instance,
            createValidator,
            updateValidator
        );
    }

    #endregion
}
