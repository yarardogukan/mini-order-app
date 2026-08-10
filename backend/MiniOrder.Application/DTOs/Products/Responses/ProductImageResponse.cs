namespace MiniOrder.Application.DTOs.Products.Responses;

public sealed record ProductImageResponse(string ImageUrl, bool IsCover, int SortOrder);
