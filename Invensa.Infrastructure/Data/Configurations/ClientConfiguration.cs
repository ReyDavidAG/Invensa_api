using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> b)
    {
        // Tabla y PK
        b.ToTable("Client");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWID()");

        // Campos básicos
        b.Property(x => x.Identifier)
            .HasMaxLength(20);

        b.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        b.Property(x => x.Email)
            .HasMaxLength(100);

        b.Property(x => x.Phone)
            .HasMaxLength(20);

        b.Property(x => x.Address)
            .HasMaxLength(250);

        b.Property(x => x.Active)
            .HasDefaultValue(true)
            .IsRequired();

        // Timestamps y concurrencia
        b.Property(x => x.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        b.Property(x => x.UpdatedAtUtc);

        b.Property(x => x.RowVersion)
            .IsRowVersion();

        // Índices
        b.HasIndex(x => x.Identifier)
            .IsUnique()
            .HasFilter("[Identifier] IS NOT NULL")
            .HasDatabaseName("UX_Client_Identifier");

        b.HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL")
            .HasDatabaseName("UX_Client_Email");

        b.HasIndex(x => x.Phone)
            .IsUnique()
            .HasFilter("[Phone] IS NOT NULL")
            .HasDatabaseName("UX_Client_Phone");

        b.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Client_Name");
    }
}
