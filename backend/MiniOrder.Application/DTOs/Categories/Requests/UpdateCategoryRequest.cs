namespace MiniOrder.Application.DTOs.Categories.Requests;

public sealed record UpdateCategoryRequest(
    string Name,
    string Slug,
    int? ParentCategoryId,
    bool IsActive
);
