using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Categories.Responses;
using MiniOrder.Application.Interfaces;
using MiniOrder.Infrastructure.Persistence;

namespace MiniOrder.Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly MiniOrderDbContext _dbContext;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(MiniOrderDbContext dbContext, ILogger<CategoryService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    #region Queries

    public async Task<Result<IReadOnlyCollection<CategoryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var categories = await _dbContext
            .Categories.AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse(category.Id, category.Name, category.Slug))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Categories retrieved. Count: {Count}", categories.Count);

        return Result<IReadOnlyCollection<CategoryResponse>>.Success(categories);
    }

    #endregion
}
