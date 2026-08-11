using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniOrder.Application.Common.Errors;
using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Categories.Requests;
using MiniOrder.Application.DTOs.Categories.Responses;
using MiniOrder.Application.Interfaces;
using MiniOrder.Domain.Entities;
using MiniOrder.Infrastructure.Persistence;

namespace MiniOrder.Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly MiniOrderDbContext _dbContext;
    private readonly ILogger<CategoryService> _logger;

    private readonly IValidator<CreateCategoryRequest> _createValidator;
    private readonly IValidator<UpdateCategoryRequest> _updateValidator;

    public CategoryService(
        MiniOrderDbContext dbContext,
        ILogger<CategoryService> logger,
        IValidator<CreateCategoryRequest> createValidator,
        IValidator<UpdateCategoryRequest> updateValidator
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    #region Commands
    public async Task<Result<CategoryDetailResponse>> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(
                " ",
                validationResult.Errors.Select(error => error.ErrorMessage)
            );

            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.ValidationFailed", errorMessage)
            );
        }

        var name = request.Name.Trim();
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.NameRequired", "Category name is required.")
            );
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.SlugRequired", "Category slug is required.")
            );
        }

        var slugExists = await _dbContext.Categories.AnyAsync(
            category => category.Slug == slug,
            cancellationToken
        );

        if (slugExists)
        {
            return Result<CategoryDetailResponse>.Failure(
                new Error(
                    "Category.SlugAlreadyExists",
                    $"Category slug '{slug}' is already in use."
                )
            );
        }

        Category? parentCategory = null;

        if (request.ParentCategoryId.HasValue)
        {
            parentCategory = await _dbContext
                .Categories.AsNoTracking()
                .FirstOrDefaultAsync(
                    category => category.Id == request.ParentCategoryId.Value,
                    cancellationToken
                );

            if (parentCategory is null)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error(
                        "Category.ParentNotFound",
                        $"Parent category with id '{request.ParentCategoryId.Value}' was not found."
                    )
                );
            }

            if (!parentCategory.IsActive)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error(
                        "Category.ParentInactive",
                        "Inactive category cannot be selected as parent."
                    )
                );
            }

            if (parentCategory.ParentCategoryId.HasValue)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error(
                        "Category.DepthExceeded",
                        "Subcategories cannot contain another subcategory."
                    )
                );
            }
        }

        var category = new Category
        {
            Name = name,
            Slug = slug,
            IsActive = true,
            ParentCategoryId = parentCategory?.Id,
        };

        await _dbContext.Categories.AddAsync(category, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Category created. CategoryId: {CategoryId}, ParentCategoryId: {ParentCategoryId}",
            category.Id,
            category.ParentCategoryId
        );

        return Result<CategoryDetailResponse>.Success(
            new CategoryDetailResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.IsActive,
                category.ParentCategoryId,
                parentCategory?.Name,
                Array.Empty<SubCategoryResponse>()
            )
        );
    }

    public async Task<Result<CategoryDetailResponse>> UpdateAsync(
        int id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(
                " ",
                validationResult.Errors.Select(error => error.ErrorMessage)
            );

            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.ValidationFailed", errorMessage)
            );
        }

        var category = await _dbContext
            .Categories.Include(category => category.SubCategories)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category is null)
        {
            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.NotFound", $"Category with id '{id}' was not found.")
            );
        }

        var name = request.Name.Trim();
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.NameRequired", "Category name is required.")
            );
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.SlugRequired", "Category slug is required.")
            );
        }

        var slugExists = await _dbContext.Categories.AnyAsync(
            existingCategory => existingCategory.Id != id && existingCategory.Slug == slug,
            cancellationToken
        );

        if (slugExists)
        {
            return Result<CategoryDetailResponse>.Failure(
                new Error(
                    "Category.SlugAlreadyExists",
                    $"Category slug '{slug}' is already in use."
                )
            );
        }

        Category? parentCategory = null;

        if (request.ParentCategoryId.HasValue)
        {
            if (request.ParentCategoryId.Value == id)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error("Category.SelfParent", "Category cannot be its own parent.")
                );
            }

            parentCategory = await _dbContext
                .Categories.AsNoTracking()
                .FirstOrDefaultAsync(
                    parent => parent.Id == request.ParentCategoryId.Value,
                    cancellationToken
                );

            if (parentCategory is null)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error(
                        "Category.ParentNotFound",
                        $"Parent category with id '{request.ParentCategoryId.Value}' was not found."
                    )
                );
            }

            if (!parentCategory.IsActive)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error(
                        "Category.ParentInactive",
                        "Inactive category cannot be selected as parent."
                    )
                );
            }

            if (parentCategory.ParentCategoryId.HasValue)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error(
                        "Category.DepthExceeded",
                        "Subcategories cannot contain another subcategory."
                    )
                );
            }

            if (category.SubCategories.Count > 0)
            {
                return Result<CategoryDetailResponse>.Failure(
                    new Error(
                        "Category.HasSubCategories",
                        "A category with subcategories cannot be moved under another category."
                    )
                );
            }
        }

        category.Name = name;
        category.Slug = slug;
        category.IsActive = request.IsActive;
        category.ParentCategoryId = parentCategory?.Id;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Category updated. CategoryId: {CategoryId}, ParentCategoryId: {ParentCategoryId}, IsActive: {IsActive}",
            category.Id,
            category.ParentCategoryId,
            category.IsActive
        );

        var activeSubCategories = category
            .SubCategories.Where(subCategory => subCategory.IsActive)
            .OrderBy(subCategory => subCategory.Name)
            .Select(subCategory => new SubCategoryResponse(
                subCategory.Id,
                subCategory.Name,
                subCategory.Slug
            ))
            .ToList();

        return Result<CategoryDetailResponse>.Success(
            new CategoryDetailResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.IsActive,
                category.ParentCategoryId,
                parentCategory?.Name,
                activeSubCategories
            )
        );
    }

    public async Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var category = await _dbContext
            .Categories.Include(category => category.SubCategories)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category is null)
        {
            return Result<bool>.Failure(
                new Error("Category.NotFound", $"Category with id '{id}' was not found.")
            );
        }

        if (!category.IsActive)
        {
            return Result<bool>.Success(true);
        }

        if (category.SubCategories.Any(subCategory => subCategory.IsActive))
        {
            return Result<bool>.Failure(
                new Error(
                    "Category.HasActiveSubCategories",
                    "Category cannot be deactivated while it has active subcategories."
                )
            );
        }

        var hasActiveProducts = await _dbContext.Products.AnyAsync(
            product => product.CategoryId == id && product.IsActive,
            cancellationToken
        );

        if (hasActiveProducts)
        {
            return Result<bool>.Failure(
                new Error(
                    "Category.HasActiveProducts",
                    "Category cannot be deactivated while it has active products."
                )
            );
        }

        category.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category deactivated. CategoryId: {CategoryId}", category.Id);

        return Result<bool>.Success(true);
    }

    #endregion

    #region Queries

    public async Task<Result<IReadOnlyCollection<CategoryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var categories = await _dbContext
            .Categories.AsNoTracking()
            .Where(category => category.IsActive && category.ParentCategoryId == null)
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Slug,
                category
                    .SubCategories.Where(subCategory => subCategory.IsActive)
                    .OrderBy(subCategory => subCategory.Name)
                    .Select(subCategory => new SubCategoryResponse(
                        subCategory.Id,
                        subCategory.Name,
                        subCategory.Slug
                    ))
                    .ToList()
            ))
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Category hierarchy retrieved. RootCount: {Count}",
            categories.Count
        );

        return Result<IReadOnlyCollection<CategoryResponse>>.Success(categories);
    }

    public async Task<Result<CategoryDetailResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var category = await _dbContext
            .Categories.AsNoTracking()
            .Where(category => category.Id == id)
            .Select(category => new CategoryDetailResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.IsActive,
                category.ParentCategoryId,
                category.ParentCategory != null ? category.ParentCategory.Name : null,
                category
                    .SubCategories.Where(subCategory => subCategory.IsActive)
                    .OrderBy(subCategory => subCategory.Name)
                    .Select(subCategory => new SubCategoryResponse(
                        subCategory.Id,
                        subCategory.Name,
                        subCategory.Slug
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            _logger.LogWarning("Category not found. CategoryId: {CategoryId}", id);

            return Result<CategoryDetailResponse>.Failure(
                new Error("Category.NotFound", $"Category with id '{id}' was not found.")
            );
        }

        _logger.LogInformation("Category retrieved. CategoryId: {CategoryId}", id);

        return Result<CategoryDetailResponse>.Success(category);
    }

    #endregion
}
