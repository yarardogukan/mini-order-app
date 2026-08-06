using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.StockCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(product => product.StockCode)
            .IsUnique();

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(product => product.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(product => product.StockQuantity)
            .IsRequired();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Products_Price_NonNegative",
                "Price >= 0");

            table.HasCheckConstraint(
                "CK_Products_StockQuantity_NonNegative",
                "StockQuantity >= 0");
        });
    }
}