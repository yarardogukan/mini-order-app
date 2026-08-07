using MiniOrder.Application.DTOs.Orders.Requests;
using MiniOrder.Application.Validators.Orders;

namespace MiniOrder.Tests.Validators;

public sealed class CreateOrderRequestValidatorTests
{
    private readonly CreateOrderRequestValidator _validator;

    public CreateOrderRequestValidatorTests()
    {
        var itemValidator = new CreateOrderItemRequestValidator();

        _validator = new CreateOrderRequestValidator(
            itemValidator);
    }

    [Fact]
    public async Task ValidateAsync_WhenItemsAreEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "Doğukan",
            []);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage ==
                "Order must contain at least one item.");
    }

    [Fact]
    public async Task ValidateAsync_WhenSameProductAddedMoreThanOnce_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "Doğukan",
            [
                new CreateOrderItemRequest(1, 1),
            new CreateOrderItemRequest(1, 2)
            ]);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage ==
                "The same product cannot be added more than once.");
    }

    [Fact]
    public async Task ValidateAsync_WhenQuantityIsZero_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateOrderRequest(
            "Doğukan",
            [
                new CreateOrderItemRequest(1, 0)
            ]);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            error => error.ErrorMessage ==
                "Quantity must be greater than zero.");
    }
}