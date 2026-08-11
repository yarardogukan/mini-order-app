using FluentValidation;
using MiniOrder.Application.DTOs.Brands.Requests;

namespace MiniOrder.Application.Validators.Brands;

public sealed class CreateBrandRequestValidator : AbstractValidator<CreateBrandRequest>
{
    public CreateBrandRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name cannot exceed 100 characters.");

        RuleFor(request => request.Slug)
            .NotEmpty()
            .WithMessage("Brand slug is required.")
            .MaximumLength(120)
            .WithMessage("Brand slug cannot exceed 120 characters.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Brand slug must contain only lowercase letters, numbers, and hyphens.");
    }
}
