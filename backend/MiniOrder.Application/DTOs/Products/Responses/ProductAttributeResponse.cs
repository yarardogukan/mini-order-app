namespace MiniOrder.Application.DTOs.Products.Responses;

public sealed record ProductAttributeResponse(
    string Name,
    string Code,
    string DataType,
    string Value,
    int SortOrder
);
