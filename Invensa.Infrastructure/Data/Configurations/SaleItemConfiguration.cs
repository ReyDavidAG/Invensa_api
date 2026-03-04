using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public sealed class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> b)
    {
        b.ToTable("SaleItem");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWID()");

        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)").IsRequired();
        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

        // Subtotal = Quantity * UnitPrice (persisted)
        b.Property(x => x.Subtotal)
            .HasColumnType("decimal(18,2)")
            .ValueGeneratedOnAddOrUpdate()
            .HasComputedColumnSql("CONVERT(decimal(18,2), [Quantity] * [UnitPrice])", stored: true);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(x => x.RowVersion)
            .HasColumnName("RowVer")
            .IsRowVersion();

        b.HasOne(x => x.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.SaleId);
        b.HasIndex(x => x.ProductId);
    }
}
