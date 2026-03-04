using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invensa.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("(newid())");
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(100).IsRequired();
        builder.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(e => e.IsAdmin).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.Active).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.CreatedAtUtc).IsRequired().HasDefaultValueSql("(sysutcdatetime())");
        
        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
    }
}
