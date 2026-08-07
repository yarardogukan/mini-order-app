namespace MiniOrder.Application.Common.Errors.BusinessErrors;

public static class OrderErrors
{
    public static readonly Error EmptyOrder = new(
        "Order.Empty",
        "Order must contain at least one item.");

    public static Error NotFound(int orderId)
    {
        return new Error(
            "Order.NotFound",
            $"Order with id '{orderId}' was not found.");
    }
}