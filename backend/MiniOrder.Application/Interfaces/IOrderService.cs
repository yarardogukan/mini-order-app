using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Orders.Requests;
using MiniOrder.Application.DTOs.Orders.Responses;

namespace MiniOrder.Application.Interfaces;

public interface IOrderService
{
    Task<Result<OrderResponse>> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<OrderResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<OrderResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}