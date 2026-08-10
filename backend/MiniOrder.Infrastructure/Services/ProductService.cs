using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MiniOrder.Application.Common.Errors.BusinessErrors;
using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Products.Responses;
using MiniOrder.Application.Interfaces;
using MiniOrder.Infrastructure.Mapping;
using MiniOrder.Infrastructure.Persistence;

namespace MiniOrder.Infrastructure.Services;

public sealed class ProductService : IProductService
{
    private readonly MiniOrderDbContext _dbContext;
    private readonly ILogger<ProductService> _logger;
    private readonly IMemoryCache _cache;

    public ProductService(
        MiniOrderDbContext dbContext,
        ILogger<ProductService> logger,
        IMemoryCache cache
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    #region Queries

    public async Task<Result<IReadOnlyCollection<ProductResponse>>> GetAllAsync(
        string? search,
        int? categoryId,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbContext
            .Products.AsNoTracking()
            .Where(product => product.IsActive && product.Category.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();

            query = query.Where(product =>
                EF.Functions.Like(product.Name, $"%{searchTerm}%")
                || EF.Functions.Like(product.StockCode, $"%{searchTerm}%")
            );
        }

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId.Value);
        }

        var products = await query
            .OrderBy(product => product.Name)
            .Select(ProductMappings.ToResponse())
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Products retrieved. Search: {Search}, CategoryId: {CategoryId}, Count: {Count}",
            search,
            categoryId,
            products.Count
        );

        return Result<IReadOnlyCollection<ProductResponse>>.Success(products);
    }

    public async Task<Result<ProductDetailResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = $"product-detail:{id}";

        if (_cache.TryGetValue(cacheKey, out ProductDetailResponse? cachedProduct))
        {
            _logger.LogInformation(
                "Product detail retrieved from cache. ProductId: {ProductId}",
                id
            );

            return Result<ProductDetailResponse>.Success(cachedProduct!);
        }

        var product = await _dbContext
            .Products.AsNoTracking()
            .Where(product =>
                product.Id == id
                && product.IsActive
                && product.Category.IsActive
                && product.Brand.IsActive
            )
            .Select(product => new ProductDetailResponse(
                product.Id,
                product.StockCode,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CategoryId,
                product.Category.Name,
                product.Category.ParentCategory != null
                    ? product.Category.ParentCategory.Name
                    : null,
                product.BrandId,
                product.Brand.Name,
                product
                    .Images.OrderBy(image => image.SortOrder)
                    .Select(image => new ProductImageResponse(
                        image.ImageUrl,
                        image.IsCover,
                        image.SortOrder
                    ))
                    .ToList(),
                product
                    .AttributeValues.OrderBy(value => value.CategoryAttribute.SortOrder)
                    .Select(value => new ProductAttributeResponse(
                        value.CategoryAttribute.Name,
                        value.CategoryAttribute.Code,
                        value.CategoryAttribute.DataType,
                        value.Value,
                        value.CategoryAttribute.SortOrder
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            _logger.LogWarning("Product detail not found. ProductId: {ProductId}", id);

            return Result<ProductDetailResponse>.Failure(ProductErrors.NotFound(id));
        }

        _cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));

        _logger.LogInformation(
            "Product detail retrieved from database and cached. ProductId: {ProductId}",
            id
        );

        return Result<ProductDetailResponse>.Success(product);
    }

    #endregion
}
