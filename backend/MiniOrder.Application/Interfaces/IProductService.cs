using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Products.Responses;

namespace MiniOrder.Application.Interfaces;

public interface IProductService
{
    Task<Result<IReadOnlyCollection<ProductResponse>>> GetAllAsync(
        string? search,
        int? categoryId,
        int? brandId,
        decimal? minPrice,
        decimal? maxPrice,
        string? sort,
        CancellationToken cancellationToken = default
    );

    Task<Result<ProductDetailResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    );
}
