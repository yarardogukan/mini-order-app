namespace MiniOrder.Application.DTOs.Products.Responses;

public sealed record ProductDetailResponse(
    int Id,
    string StockCode,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    int CategoryId,
    string CategoryName,
    string? ParentCategoryName,
    int BrandId,
    string BrandName,
    IReadOnlyCollection<ProductImageResponse> Images,
    IReadOnlyCollection<ProductAttributeResponse> Attributes
);
