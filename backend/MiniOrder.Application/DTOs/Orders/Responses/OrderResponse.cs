namespace MiniOrder.Application.DTOs.Orders.Responses;

public sealed record OrderResponse(
    int Id,
    string CustomerName,
    DateTimeOffset CreatedAt,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemResponse> Items);