using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Brands.Requests;
using MiniOrder.Application.DTOs.Brands.Responses;

namespace MiniOrder.Application.Interfaces;

public interface IBrandService
{
    Task<Result<IReadOnlyCollection<BrandResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default
    );

    Task<Result<BrandDetailResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    );

    Task<Result<BrandDetailResponse>> CreateAsync(
        CreateBrandRequest request,
        CancellationToken cancellationToken = default
    );

    Task<Result<BrandDetailResponse>> UpdateAsync(
        int id,
        UpdateBrandRequest request,
        CancellationToken cancellationToken = default
    );

    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
