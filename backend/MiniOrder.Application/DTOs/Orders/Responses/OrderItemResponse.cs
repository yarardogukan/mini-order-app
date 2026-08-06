namespace MiniOrder.Application.DTOs.Orders.Responses;

public sealed record OrderItemResponse(
    int ProductId,
    string ProductName,
    string StockCode,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);