using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Persistence.Configurations;

public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.HasKey(brand => brand.Id);

        builder.Property(brand => brand.Name).IsRequired().HasMaxLength(100);

        builder.Property(brand => brand.Slug).IsRequired().HasMaxLength(120);

        builder.HasIndex(brand => brand.Slug).IsUnique();

        builder.Property(brand => brand.IsActive).IsRequired();
    }
}
