using Invensa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invensa.Infrastructure.Data;

public class InvensaDbContext : DbContext
{
    public InvensaDbContext(DbContextOptions<InvensaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Sale> Sales { get; set; }
    public virtual DbSet<SaleItem> SaleItems { get; set; }
    public virtual DbSet<Unit> Units { get; set; }
    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }
    public virtual DbSet<ImageFile> ImageFiles { get; set; }
    public virtual DbSet<Client> Clients { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvensaDbContext).Assembly);
    }
}
