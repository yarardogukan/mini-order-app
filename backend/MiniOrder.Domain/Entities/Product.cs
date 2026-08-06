namespace MiniOrder.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string StockCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}