namespace MiniOrder.Application.DTOs.Categories.Responses;

public sealed record CategoryDetailResponse(
    int Id,
    string Name,
    string Slug,
    bool IsActive,
    int? ParentCategoryId,
    string? ParentCategoryName,
    IReadOnlyCollection<SubCategoryResponse> SubCategories
);
