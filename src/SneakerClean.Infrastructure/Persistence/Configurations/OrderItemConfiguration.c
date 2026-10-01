
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SneakerClean.Domain.Entities;

namespace SneakerClean.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SneakerBrand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.SneakerModel)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.ServiceDescription)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(i => i.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}