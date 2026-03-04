namespace Invensa.Infrastructure.Repositories;

using Domain.Interfaces;
using Invensa.Domain.Entities;
using Invensa.Domain.Exceptions;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class UnitOfWork : IUnitOfWork
{
    private readonly InvensaDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    public UnitOfWork(InvensaDbContext context, ILogger<UnitOfWork> logger, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, 
        IProductRepository productRepository, IUnitRepository unitRepository, IRepository<InventoryMovement> inventoryMovementRepository, IRepository<SaleItem> saleItemRepository,
        ISaleRepository saleRepository, IClientRepository clientRepository)
    {
        _context = context;
        _logger = logger;
        UserRepository = userRepository;
        RefreshTokenRepository = refreshTokenRepository;
        ProductRepository = productRepository;
        InventoryMovementRepository = inventoryMovementRepository;
        UnitRepository = unitRepository;
        SaleItemRepository = saleItemRepository;
        SaleRepository = saleRepository;
        ClientRepository = clientRepository;
    }
    public IRefreshTokenRepository RefreshTokenRepository { get; }
    public IUserRepository UserRepository { get; }
    public IUnitRepository UnitRepository { get; set; }
    public IProductRepository ProductRepository { get; set; }
    public IRepository<InventoryMovement> InventoryMovementRepository { get; set; }
    public IRepository<SaleItem> SaleItemRepository { get; set; }
    public ISaleRepository SaleRepository { get; set; }
    public IClientRepository ClientRepository { get; }


    public bool SaveChanges()
    {
        try
        {
            _logger.LogInformation("Commiting changes.");

            var saved = _context.SaveChanges();

            _logger.LogInformation("Saved changes: {saved}", saved);

            return saved > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception has ocurred {Message}.", ex.Message);

            throw new InfrastructureException(ex.Message);
        }
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var changes = await _context.SaveChangesAsync(cancellationToken);
            return changes > 0;
        }
        catch (DbUpdateException ex)
        {
            // LOG con máximo detalle
            var inner = ex.InnerException?.Message ?? ex.Message;
            _logger.LogError(ex, "DB Update failed. Inner: {Inner}", inner);

            // Re-propaga con detalle (temporalmente) para que lo veas en Swagger
            throw new InfrastructureException($"DB error: {inner}");
        }
    }

    public void SetOriginalRowVersion<TEntity>(TEntity entity, byte[] clientRowVersion) where TEntity : class
    {
        var entry = _context.Entry(entity);
        // El nombre debe coincidir con tu propiedad RowVersion en la entidad
        entry.Property("RowVersion").OriginalValue = clientRowVersion;
    }

}
