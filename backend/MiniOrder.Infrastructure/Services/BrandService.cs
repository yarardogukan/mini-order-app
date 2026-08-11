using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniOrder.Application.Common.Errors;
using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Brands.Requests;
using MiniOrder.Application.DTOs.Brands.Responses;
using MiniOrder.Application.Interfaces;
using MiniOrder.Domain.Entities;
using MiniOrder.Infrastructure.Persistence;

namespace MiniOrder.Infrastructure.Services;

public sealed class BrandService : IBrandService
{
    private readonly MiniOrderDbContext _dbContext;
    private readonly ILogger<BrandService> _logger;

    private readonly IValidator<CreateBrandRequest> _createValidator;
    private readonly IValidator<UpdateBrandRequest> _updateValidator;

    public BrandService(
        MiniOrderDbContext dbContext,
        ILogger<BrandService> logger,
        IValidator<CreateBrandRequest> createValidator,
        IValidator<UpdateBrandRequest> updateValidator
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IReadOnlyCollection<BrandResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var brands = await _dbContext
            .Brands.AsNoTracking()
            .Where(brand => brand.IsActive)
            .OrderBy(brand => brand.Name)
            .Select(brand => new BrandResponse(brand.Id, brand.Name, brand.Slug))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Brands retrieved. Count: {Count}", brands.Count);

        return Result<IReadOnlyCollection<BrandResponse>>.Success(brands);
    }

    public async Task<Result<BrandDetailResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var brand = await _dbContext
            .Brands.AsNoTracking()
            .Where(brand => brand.Id == id)
            .Select(brand => new BrandDetailResponse(
                brand.Id,
                brand.Name,
                brand.Slug,
                brand.IsActive
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (brand is null)
        {
            _logger.LogWarning("Brand not found. BrandId: {BrandId}", id);

            return Result<BrandDetailResponse>.Failure(
                new Error("Brand.NotFound", $"Brand with id '{id}' was not found.")
            );
        }

        _logger.LogInformation("Brand retrieved. BrandId: {BrandId}", id);

        return Result<BrandDetailResponse>.Success(brand);
    }

    public async Task<Result<BrandDetailResponse>> CreateAsync(
        CreateBrandRequest request,
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

            return Result<BrandDetailResponse>.Failure(
                new Error("Brand.ValidationFailed", errorMessage)
            );
        }

        var name = request.Name.Trim();
        var slug = request.Slug.Trim().ToLowerInvariant();

        var slugExists = await _dbContext.Brands.AnyAsync(
            brand => brand.Slug == slug,
            cancellationToken
        );

        if (slugExists)
        {
            return Result<BrandDetailResponse>.Failure(
                new Error("Brand.SlugAlreadyExists", $"Brand slug '{slug}' is already in use.")
            );
        }

        var brand = new Brand
        {
            Name = name,
            Slug = slug,
            IsActive = true,
        };

        await _dbContext.Brands.AddAsync(brand, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Brand created. BrandId: {BrandId}", brand.Id);

        return Result<BrandDetailResponse>.Success(
            new BrandDetailResponse(brand.Id, brand.Name, brand.Slug, brand.IsActive)
        );
    }

    public async Task<Result<BrandDetailResponse>> UpdateAsync(
        int id,
        UpdateBrandRequest request,
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

            return Result<BrandDetailResponse>.Failure(
                new Error("Brand.ValidationFailed", errorMessage)
            );
        }

        var brand = await _dbContext.Brands.FirstOrDefaultAsync(
            brand => brand.Id == id,
            cancellationToken
        );

        if (brand is null)
        {
            return Result<BrandDetailResponse>.Failure(
                new Error("Brand.NotFound", $"Brand with id '{id}' was not found.")
            );
        }

        var name = request.Name.Trim();
        var slug = request.Slug.Trim().ToLowerInvariant();

        var slugExists = await _dbContext.Brands.AnyAsync(
            existingBrand => existingBrand.Id != id && existingBrand.Slug == slug,
            cancellationToken
        );

        if (slugExists)
        {
            return Result<BrandDetailResponse>.Failure(
                new Error("Brand.SlugAlreadyExists", $"Brand slug '{slug}' is already in use.")
            );
        }

        brand.Name = name;
        brand.Slug = slug;
        brand.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Brand updated. BrandId: {BrandId}, IsActive: {IsActive}",
            brand.Id,
            brand.IsActive
        );

        return Result<BrandDetailResponse>.Success(
            new BrandDetailResponse(brand.Id, brand.Name, brand.Slug, brand.IsActive)
        );
    }

    public async Task<Result<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var brand = await _dbContext.Brands.FirstOrDefaultAsync(
            brand => brand.Id == id,
            cancellationToken
        );

        if (brand is null)
        {
            return Result<bool>.Failure(
                new Error("Brand.NotFound", $"Brand with id '{id}' was not found.")
            );
        }

        if (!brand.IsActive)
        {
            return Result<bool>.Success(true);
        }

        var hasActiveProducts = await _dbContext.Products.AnyAsync(
            product => product.BrandId == id && product.IsActive,
            cancellationToken
        );

        if (hasActiveProducts)
        {
            return Result<bool>.Failure(
                new Error(
                    "Brand.HasActiveProducts",
                    "Brand cannot be deactivated while it has active products."
                )
            );
        }

        brand.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Brand deactivated. BrandId: {BrandId}", brand.Id);

        return Result<bool>.Success(true);
    }
}
