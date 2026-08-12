using FluentValidation;
using MiniOrder.Application.DTOs.Carts.Requests;

namespace MiniOrder.Application.Validators.Carts;

public sealed class UpdateCartItemRequestValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemRequestValidator()
    {
        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}
