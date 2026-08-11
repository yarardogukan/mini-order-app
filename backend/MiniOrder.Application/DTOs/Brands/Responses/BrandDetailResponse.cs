namespace MiniOrder.Application.DTOs.Brands.Responses;

public sealed record BrandDetailResponse(int Id, string Name, string Slug, bool IsActive);
