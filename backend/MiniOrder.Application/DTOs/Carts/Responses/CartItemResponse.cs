namespace MiniOrder.Application.DTOs.Carts.Responses;

public class CartItemResponse
{
    public int ProductId { get; set; }

    public string StockCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string BrandName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }
}
