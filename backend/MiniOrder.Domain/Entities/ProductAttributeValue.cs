namespace MiniOrder.Domain.Entities;

public class ProductAttributeValue
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int CategoryAttributeId { get; set; }

    public string Value { get; set; } = string.Empty;

    public Product Product { get; set; } = null!;

    public CategoryAttribute CategoryAttribute { get; set; } = null!;
}
