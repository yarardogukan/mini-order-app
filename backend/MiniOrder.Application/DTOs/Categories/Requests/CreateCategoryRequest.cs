namespace MiniOrder.Application.DTOs.Categories.Requests;

public sealed record CreateCategoryRequest(string Name, string Slug, int? ParentCategoryId);
