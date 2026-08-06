using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Products.Responses;

namespace MiniOrder.Application.Interfaces;

public interface IProductService
{
    Task<Result<IReadOnlyCollection<ProductResponse>>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<Result<ProductResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}