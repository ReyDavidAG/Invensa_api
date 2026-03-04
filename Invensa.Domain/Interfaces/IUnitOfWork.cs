namespace Invensa.Domain.Interfaces;

using Entities;

public interface IUnitOfWork
{
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IUserRepository UserRepository { get; }
    IProductRepository ProductRepository { get; }
    IUnitRepository UnitRepository { get; }
    IRepository<InventoryMovement> InventoryMovementRepository { get; }
    IRepository<SaleItem> SaleItemRepository { get; }
    ISaleRepository SaleRepository { get; }
    IClientRepository ClientRepository { get; }
    bool SaveChanges();
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    void SetOriginalRowVersion<TEntity>(TEntity entity, byte[] clientRowVersion) where TEntity : class;
}
