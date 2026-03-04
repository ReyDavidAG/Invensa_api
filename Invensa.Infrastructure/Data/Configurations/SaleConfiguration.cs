using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> b)
    {
        b.ToTable("Sale");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWID()");

        b.Property(x => x.DateUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(x => x.Total).HasColumnType("decimal(18,2)").HasDefaultValue(0).IsRequired();

        // TicketNumber identity -> se configura en migración/SQL (Identity)
        b.Property(x => x.TicketNumber).ValueGeneratedOnAdd();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(x => x.UpdatedAtUtc);
        b.Property(x => x.RowVersion)
            .HasColumnName("RowVer")
            .IsRowVersion();

        // Relación con Client
        b.HasOne(x => x.Client)
            .WithMany(x => x.Sales) 
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.ClientId)
            .HasDatabaseName("IX_Sale_ClientId");
    }
}
