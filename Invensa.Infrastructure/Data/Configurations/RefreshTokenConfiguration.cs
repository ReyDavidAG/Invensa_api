using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("(newid())");
        
        builder.Property(e => e.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.ExpiresAtUtc)
            .IsRequired();

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("(sysutcdatetime())");

        // Relación con User
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
