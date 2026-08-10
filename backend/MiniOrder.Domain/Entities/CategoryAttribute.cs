namespace MiniOrder.Domain.Entities;

public class CategoryAttribute
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public bool IsRequired { get; set; }

    public int SortOrder { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } =
        new List<ProductAttributeValue>();
}
