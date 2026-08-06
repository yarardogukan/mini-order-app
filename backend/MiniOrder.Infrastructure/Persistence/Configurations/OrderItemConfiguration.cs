using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(orderItem => orderItem.Id);

        builder.Property(orderItem => orderItem.Quantity)
            .IsRequired();

        builder.Property(orderItem => orderItem.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(orderItem => orderItem.LineTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(orderItem => orderItem.Product)
            .WithMany(product => product.OrderItems)
            .HasForeignKey(orderItem => orderItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(orderItem => orderItem.OrderId);

        builder.HasIndex(orderItem => orderItem.ProductId);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_OrderItems_Quantity_Positive",
                "Quantity > 0");

            table.HasCheckConstraint(
                "CK_OrderItems_UnitPrice_NonNegative",
                "UnitPrice >= 0");

            table.HasCheckConstraint(
                "CK_OrderItems_LineTotal_NonNegative",
                "LineTotal >= 0");
        });
    }
}