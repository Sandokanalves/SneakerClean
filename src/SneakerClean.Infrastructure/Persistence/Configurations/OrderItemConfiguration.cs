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
        builder.Property(i => i.OrderId).IsRequired();
        builder.Property(i => i.SneakerBrand).IsRequired().HasMaxLength(100);
        builder.Property(i => i.SneakerModel).IsRequired().HasMaxLength(150);
        builder.Property(i => i.Color).HasMaxLength(80);
        builder.Property(i => i.Size).IsRequired();
        builder.Property(i => i.ServiceDescription).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Price).IsRequired().HasColumnType("decimal(10,2)");
        builder.Property(i => i.Notes).HasMaxLength(500);
    }
}
