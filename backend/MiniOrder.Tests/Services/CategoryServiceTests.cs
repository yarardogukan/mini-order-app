using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiniOrder.Application.DTOs.Categories.Requests;
using MiniOrder.Application.Validators.Categories;
using MiniOrder.Domain.Entities;
using MiniOrder.Infrastructure.Persistence;
using MiniOrder.Infrastructure.Services;

namespace MiniOrder.Tests.Services;

public sealed class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ShouldCreateCategory()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            var service = CreateCategoryService(dbContext);

            var request = new CreateCategoryRequest("Gaming", "gaming", null);

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal("Gaming", result.Value.Name);
            Assert.Equal("gaming", result.Value.Slug);
            Assert.True(result.Value.IsActive);
            Assert.Null(result.Value.ParentCategoryId);
            Assert.Null(result.Value.ParentCategoryName);
            Assert.Empty(result.Value.SubCategories);

            var category = await dbContext.Categories.SingleAsync();

            Assert.Equal("Gaming", category.Name);
            Assert.Equal("gaming", category.Slug);
            Assert.True(category.IsActive);
            Assert.Null(category.ParentCategoryId);
        }
    }

    [Fact]
    public async Task CreateAsync_WhenSlugAlreadyExists_ShouldFailAndNotCreateCategory()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            dbContext.Categories.Add(
                new Category
                {
                    Name = "Computers",
                    Slug = "computers",
                    IsActive = true,
                }
            );

            await dbContext.SaveChangesAsync();

            var service = CreateCategoryService(dbContext);

            var request = new CreateCategoryRequest("Another Computers", "computers", null);

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Category.SlugAlreadyExists", result.Error.Code);

            var categories = await dbContext.Categories.ToListAsync();

            Assert.Single(categories);
            Assert.Equal("Computers", categories[0].Name);
        }
    }

    [Fact]
    public async Task CreateAsync_WhenParentCategoryIsValid_ShouldCreateSubCategory()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            var parentCategory = new Category
            {
                Name = "Computers",
                Slug = "computers",
                IsActive = true,
            };

            dbContext.Categories.Add(parentCategory);
            await dbContext.SaveChangesAsync();

            var service = CreateCategoryService(dbContext);

            var request = new CreateCategoryRequest("Tablets", "tablets", parentCategory.Id);

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal("Tablets", result.Value.Name);
            Assert.Equal("tablets", result.Value.Slug);
            Assert.Equal(parentCategory.Id, result.Value.ParentCategoryId);
            Assert.Equal("Computers", result.Value.ParentCategoryName);

            var subCategory = await dbContext.Categories.SingleAsync(category =>
                category.Slug == "tablets"
            );

            Assert.Equal(parentCategory.Id, subCategory.ParentCategoryId);
            Assert.True(subCategory.IsActive);
        }
    }

    [Fact]
    public async Task CreateAsync_WhenParentIsSubCategory_ShouldFailWithDepthExceeded()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            var rootCategory = new Category
            {
                Name = "Computers",
                Slug = "computers",
                IsActive = true,
            };

            dbContext.Categories.Add(rootCategory);
            await dbContext.SaveChangesAsync();

            var subCategory = new Category
            {
                Name = "Laptops",
                Slug = "laptops",
                IsActive = true,
                ParentCategoryId = rootCategory.Id,
            };

            dbContext.Categories.Add(subCategory);
            await dbContext.SaveChangesAsync();

            var service = CreateCategoryService(dbContext);

            var request = new CreateCategoryRequest(
                "Gaming Laptops",
                "gaming-laptops",
                subCategory.Id
            );

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Category.DepthExceeded", result.Error.Code);

            var gamingLaptopsExists = await dbContext.Categories.AnyAsync(category =>
                category.Slug == "gaming-laptops"
            );

            Assert.False(gamingLaptopsExists);
        }
    }

    [Fact]
    public async Task UpdateAsync_WhenRequestIsValid_ShouldUpdateCategory()
    {
        // Arrange
        var (connection, dbContext) = await CreateDbContextAsync();

        await using (connection)
        await using (dbContext)
        {
            var category = new Category
            {
                Name = "Old Name",
                Slug = "old-name",
                IsActive = true,
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var service = CreateCategoryService(dbContext);

            var request = new UpdateCategoryRequest("New Name", "new-name", null, true);

            // Act
            var result = await service.UpdateAsync(category.Id, request);

            // Assert
            Assert.True(result.IsSuccess);

            Assert.Equal("New Name", result.Value.Name);
            Assert.Equal("new-name", result.Value.Slug);
            Assert.True(result.Value.IsActive);
            Assert.Null(result.Value.ParentCategoryId);

            var updatedCategory = await dbContext.Categories.SingleAsync(existingCategory =>
                existingCategory.Id == category.Id
            );

            Assert.Equal("New Name", updatedCategory.Name);
            Assert.Equal("new-name", updatedCategory.Slug);
            Assert.True(updatedCategory.IsActive);
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryHasActiveProducts_ShouldFail()
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

            var service = CreateCategoryService(dbContext);

            // Act
            var result = await service.DeleteAsync(category.Id);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Category.HasActiveProducts", result.Error.Code);

            var storedCategory = await dbContext.Categories.SingleAsync(existingCategory =>
                existingCategory.Id == category.Id
            );

            Assert.True(storedCategory.IsActive);
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

    private static CategoryService CreateCategoryService(MiniOrderDbContext dbContext)
    {
        IValidator<CreateCategoryRequest> createValidator = new CreateCategoryRequestValidator();

        IValidator<UpdateCategoryRequest> updateValidator = new UpdateCategoryRequestValidator();

        return new CategoryService(
            dbContext,
            NullLogger<CategoryService>.Instance,
            createValidator,
            updateValidator
        );
    }

    #endregion
}
