namespace MiniOrder.Application.DTOs.Brands.Requests;

public sealed record UpdateBrandRequest(string Name, string Slug, bool IsActive);
