using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

internal class ImageFilesConfiguration : IEntityTypeConfiguration<ImageFiles>
{
    public void Configure(EntityTypeBuilder<ImageFiles> builder)
    {
        builder.ToTable("ImageFiles");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("(newid())");

        builder.Property(e => e.OriginalFileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.ContentType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.Sha256)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.StoragePath)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("(sysutcdatetime())");
    }
}
