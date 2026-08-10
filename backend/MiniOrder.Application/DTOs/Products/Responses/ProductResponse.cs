namespace MiniOrder.Application.DTOs.Products.Responses;

public sealed record ProductResponse(
    int Id,
    string StockCode,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    int CategoryId,
    string CategoryName
);
