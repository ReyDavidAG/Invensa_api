namespace Invensa.Infrastructure.Data.Configurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class Auth0TokenConfiguration: IEntityTypeConfiguration<Auth0Token>
{
    public void Configure(EntityTypeBuilder<Auth0Token> builder)
    {
        builder.ToTable("Auth0Token");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.ExpirationDate).IsRequired();
    }
}