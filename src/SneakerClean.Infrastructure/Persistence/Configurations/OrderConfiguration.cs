using Microsoft.EntityFrameworkCore;
using SneakerClean.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SneakerClean.Domain.ValueObjects;

namespace SneakerClean.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(o => o.OrderCode).IsUnique();
        builder.Property(o => o.CustomerId).IsRequired();
        builder.Property(o => o.Status).IsRequired().HasConversion<int>();
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);

        // Mapeamento do Value Object (Address) como Owned Entity (Tabela embutida)
        builder.OwnsOne(o => o.DeliveryAddress, addr =>
        {
            addr.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200);
            addr.Property(a => a.Number).HasColumnName("Number").HasMaxLength(20);
            addr.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
            addr.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
            addr.Property(a => a.State).HasColumnName("State").HasMaxLength(50);
        });

        // Relacionamento com Customer (Navigation)
        builder.HasOne(o => o.Customer)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento 1:N com OrderItem
        builder.HasMany(o => o.Items)
               .WithOne()
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}