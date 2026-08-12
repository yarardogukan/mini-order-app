namespace MiniOrder.Application.DTOs.Carts.Requests;

public class AddCartItemRequest
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
