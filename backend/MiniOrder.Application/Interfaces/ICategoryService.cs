using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Categories.Responses;

namespace MiniOrder.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<IReadOnlyCollection<CategoryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default
    );
}
