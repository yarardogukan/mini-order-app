namespace MiniOrder.Application.DTOs.Orders.Requests;

public sealed record CreateOrderRequest(
    string CustomerName,
    IReadOnlyCollection<CreateOrderItemRequest> Items);