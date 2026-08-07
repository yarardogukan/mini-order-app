using Microsoft.EntityFrameworkCore;
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

    public ProductService(
        MiniOrderDbContext dbContext,
        ILogger<ProductService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    #region Queries

    public async Task<Result<IReadOnlyCollection<ProductResponse>>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();

            query = query.Where(product =>
                EF.Functions.Like(product.Name, $"%{searchTerm}%") ||
                EF.Functions.Like(product.StockCode, $"%{searchTerm}%"));
        }

        var products = await query
            .OrderBy(product => product.Name)
            .Select(ProductMappings.ToResponse())
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Products retrieved. Search: {Search}, Count: {Count}",
            search,
            products.Count);

        return Result<IReadOnlyCollection<ProductResponse>>
            .Success(products);
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(ProductMappings.ToResponse())
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            _logger.LogWarning(
                "Product not found. ProductId: {ProductId}",
                id);

            return Result<ProductResponse>.Failure(
                ProductErrors.NotFound(id));
        }

        _logger.LogInformation(
            "Product retrieved successfully. ProductId: {ProductId}",
            id);

        return Result<ProductResponse>.Success(product);
    }

    #endregion
}