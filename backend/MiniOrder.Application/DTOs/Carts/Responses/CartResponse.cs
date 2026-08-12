namespace MiniOrder.Application.DTOs.Carts.Responses;

public class CartResponse
{
    public Guid CartId { get; set; }

    public int ItemCount { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Total { get; set; }

    public IReadOnlyCollection<CartItemResponse> Items { get; set; } =
        Array.Empty<CartItemResponse>();
}
