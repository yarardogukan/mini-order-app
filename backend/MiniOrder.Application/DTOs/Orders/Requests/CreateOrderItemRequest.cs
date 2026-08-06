namespace MiniOrder.Application.DTOs.Orders.Requests;

public sealed record CreateOrderItemRequest(
    int ProductId,
    int Quantity);