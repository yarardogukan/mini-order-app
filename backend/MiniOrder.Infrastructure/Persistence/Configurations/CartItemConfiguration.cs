using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Configurations;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(
            "CartItems",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_CartItems_Quantity_Positive", "Quantity > 0");
            }
        );

        builder.HasKey(cartItem => cartItem.Id);

        builder.Property(cartItem => cartItem.Quantity).IsRequired();

        builder
            .HasOne(cartItem => cartItem.Product)
            .WithMany(product => product.CartItems)
            .HasForeignKey(cartItem => cartItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cartItem => new { cartItem.CartId, cartItem.ProductId }).IsUnique();
    }
}
