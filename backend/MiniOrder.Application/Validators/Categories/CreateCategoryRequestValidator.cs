using FluentValidation;
using MiniOrder.Application.DTOs.Categories.Requests;

namespace MiniOrder.Application.Validators.Categories;

public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(request => request.Slug)
            .NotEmpty()
            .WithMessage("Category slug is required.")
            .MaximumLength(120)
            .WithMessage("Category slug cannot exceed 120 characters.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage(
                "Category slug must contain only lowercase letters, numbers, and hyphens."
            );

        RuleFor(request => request.ParentCategoryId)
            .GreaterThan(0)
            .When(request => request.ParentCategoryId.HasValue)
            .WithMessage("Parent category id must be greater than zero.");
    }
}
