namespace MiniOrder.Application.DTOs.Products.Responses;

public sealed record ProductResponse(
    int Id,
    string StockCode,
    string Name,
    decimal Price,
    int StockQuantity);