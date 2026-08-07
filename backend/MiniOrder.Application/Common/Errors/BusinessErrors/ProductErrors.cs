namespace MiniOrder.Application.Common.Errors.BusinessErrors;

public static class ProductErrors
{
    public static Error NotFound(int productId)
    {
        return new Error(
            "Product.NotFound",
            $"Product with id '{productId}' was not found.");
    }

    public static Error InsufficientStock(
        string productName,
        int requestedQuantity,
        int availableQuantity)
    {
        return new Error(
            "Product.InsufficientStock",
            $"'{productName}' has insufficient stock. Requested: {requestedQuantity}, Available: {availableQuantity}.");
    }
}