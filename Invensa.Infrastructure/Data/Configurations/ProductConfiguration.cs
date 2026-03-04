using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        // Tabla y PK
        b.ToTable("Product");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWID()");

        // Campos básicos
        b.Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        b.Property(x => x.Code)
            .HasMaxLength(64)
            .IsRequired();

        b.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("UX_Product_Code");

        b.Property(x => x.PriceSale)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0)
            .IsRequired();

        b.Property(x => x.PriceBuy)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0)
            .IsRequired();

        // Timestamps y concurrencia
        b.Property(x => x.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        b.Property(x => x.UpdatedAtUtc);

        b.Property(x => x.RowVersion)
            .IsRowVersion();

        // FK con Unit (como ya lo tenías)
        b.HasOne(x => x.Unit)
            .WithMany(u => u.Products)
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices útiles
        b.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Product_Name");

        b.HasIndex(x => x.UnitId)
            .HasDatabaseName("IX_Product_UnitId");

        // ---------- Imagen opcional ----------
        // Si tu entidad Product tiene: public Guid? ImageId { get; set; }
        b.Property(x => x.ImageId)
            .IsRequired(false);

        // Relación 1:1 lógica con ImageFile mediante índice único filtrado
        // (No definimos navegación en Product ni en ImageFile, usamos tipo genérico)
        b.HasOne<ImageFile>()
            .WithMany()
            .HasForeignKey(p => p.ImageId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Product_ImageFiles_ImageId");

        // Unique index filtrado para permitir sólo una referencia por imagen (1:1 lógico)
        b.HasIndex(p => p.ImageId)
            .IsUnique()
            .HasFilter("[ImageId] IS NOT NULL")
            .HasDatabaseName("UX_Product_ImageId_NotNull");
    }
}
