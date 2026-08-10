using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Configurations;

public sealed class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
{
    public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
    {
        builder.ToTable("CategoryAttributes");

        builder.HasKey(attribute => attribute.Id);

        builder.Property(attribute => attribute.Name).IsRequired().HasMaxLength(100);

        builder.Property(attribute => attribute.Code).IsRequired().HasMaxLength(100);

        builder.Property(attribute => attribute.DataType).IsRequired().HasMaxLength(30);

        builder.Property(attribute => attribute.IsRequired).IsRequired();

        builder.Property(attribute => attribute.SortOrder).IsRequired();

        builder.HasIndex(attribute => new { attribute.CategoryId, attribute.Code }).IsUnique();

        builder
            .HasOne(attribute => attribute.Category)
            .WithMany(category => category.Attributes)
            .HasForeignKey(attribute => attribute.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
