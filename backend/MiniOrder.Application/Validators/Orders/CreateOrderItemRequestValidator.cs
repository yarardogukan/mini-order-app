using FluentValidation;
using MiniOrder.Application.DTOs.Orders.Requests;

namespace MiniOrder.Application.Validators.Orders;

public sealed class CreateOrderItemRequestValidator
    : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(item => item.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be greater than zero.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}