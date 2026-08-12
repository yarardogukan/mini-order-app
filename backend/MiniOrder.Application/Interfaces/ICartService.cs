using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Carts.Requests;
using MiniOrder.Application.DTOs.Carts.Responses;

namespace MiniOrder.Application.Interfaces;

public interface ICartService
{
    Task<Result<CartResponse>> GetCartAsync(
        Guid cartId,
        CancellationToken cancellationToken = default
    );

    Task<Result<CartResponse>> AddItemAsync(
        Guid? cartId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default
    );

    Task<Result<CartResponse>> UpdateItemQuantityAsync(
        Guid cartId,
        int productId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default
    );

    Task<Result<CartResponse>> RemoveItemAsync(
        Guid cartId,
        int productId,
        CancellationToken cancellationToken = default
    );

    Task<Result<bool>> ClearAsync(Guid cartId, CancellationToken cancellationToken = default);
}
