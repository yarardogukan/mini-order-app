using FluentValidation;
using MiniOrder.Application.DTOs.Orders.Requests;

namespace MiniOrder.Application.Validators.Orders;

public sealed class CreateOrderRequestValidator
    : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator(
        IValidator<CreateOrderItemRequest> itemValidator)
    {
        RuleFor(request => request.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(150)
            .WithMessage("Customer name cannot exceed 150 characters.");

        RuleFor(request => request.Items)
            .NotNull()
            .WithMessage("Order items are required.")
            .Must(items => items is { Count: > 0 })
            .WithMessage("Order must contain at least one item.");

        RuleFor(request => request.Items)
            .Must(items =>
                items is null ||
                items.Select(item => item.ProductId).Distinct().Count() == items.Count)
            .WithMessage("The same product cannot be added more than once.");

        RuleForEach(request => request.Items)
            .SetValidator(itemValidator);
    }
}