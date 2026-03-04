using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public sealed class ImageFileConfiguration : IEntityTypeConfiguration<ImageFile>
{
    public void Configure(EntityTypeBuilder<ImageFile> b)
    {
        b.ToTable("ImageFiles", "dbo");

        b.HasKey(x => x.Id).HasName("PK_ImageFiles");
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(256);
        b.Property(x => x.ContentType).IsRequired().HasMaxLength(128);
        b.Property(x => x.SizeBytes).IsRequired();
        b.Property(x => x.Sha256).IsRequired().HasMaxLength(64);
        b.Property(x => x.StoragePath).IsRequired().HasMaxLength(512);

        b.Property(x => x.CreatedAtUtc)
         .IsRequired()
         .HasColumnType("datetime2(7)")
         .HasDefaultValueSql("SYSUTCDATETIME()");

        b.HasIndex(x => x.Sha256).IsUnique().HasDatabaseName("UX_ImageFiles_Sha256");
        b.HasIndex(x => x.StoragePath).HasDatabaseName("IX_ImageFiles_StoragePath");
    }
}
