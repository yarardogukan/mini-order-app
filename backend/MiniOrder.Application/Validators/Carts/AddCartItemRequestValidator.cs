using FluentValidation;
using MiniOrder.Application.DTOs.Carts.Requests;

namespace MiniOrder.Application.Validators.Carts;

public sealed class AddCartItemRequestValidator : AbstractValidator<AddCartItemRequest>
{
    public AddCartItemRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0)
            .WithMessage("Product id must be greater than zero.");

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}
