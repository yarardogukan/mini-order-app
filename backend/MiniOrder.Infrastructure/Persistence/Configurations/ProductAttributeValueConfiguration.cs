using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Configurations;

public sealed class ProductAttributeValueConfiguration
    : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");

        builder.HasKey(value => value.Id);

        builder.Property(value => value.Value).IsRequired().HasMaxLength(500);

        builder.HasIndex(value => new { value.ProductId, value.CategoryAttributeId }).IsUnique();

        builder
            .HasOne(value => value.Product)
            .WithMany(product => product.AttributeValues)
            .HasForeignKey(value => value.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(value => value.CategoryAttribute)
            .WithMany(attribute => attribute.ProductAttributeValues)
            .HasForeignKey(value => value.CategoryAttributeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
