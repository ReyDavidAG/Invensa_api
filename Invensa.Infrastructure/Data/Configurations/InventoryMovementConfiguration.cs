using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> b)
    {
        b.ToTable("InventoryMovement");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWID()");

        b.Property(x => x.MovementType).IsRequired();

        b.Property(x => x.Quantity).HasColumnType("decimal(18,4)");
        b.Property(x => x.QuantityAdj).HasColumnType("decimal(18,4)");
        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");

        b.Property(x => x.Note).HasMaxLength(200);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        b.HasOne(x => x.Product)
            .WithMany(p => p.InventoryMovements)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Sale)
            .WithMany(s => s.Movements) // relación lógica: salida por venta (no estricta)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasIndex(x => x.ProductId);
        b.HasIndex(x => x.MovementType);
        b.HasIndex(x => x.SaleId);

        // Validaciones de negocio a nivel DB (equivalentes al CHECK propuesto)
        // EF no crea CHECK por defecto; puedes agregarlos en migración:
        // migrationBuilder.Sql("ALTER TABLE ... ADD CONSTRAINT CK_InvMov_Qty_Positive ...");
    }
}
