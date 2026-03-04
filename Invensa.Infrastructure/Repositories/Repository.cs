namespace Invensa.Infrastructure.Repositories;

using System.Linq.Expressions;
using Domain.Exceptions;
using Domain.Interfaces;
using Invensa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class Repository<T> : IRepository<T> where T : class
{
    private const string DefaultDatabaseErrorMessage = "A database communication error has occurred.";
    private readonly InvensaDbContext Context;
    protected readonly DbSet<T> Entities;
    protected readonly ILogger<Repository<T>> Logger;

    // ✅ ÚNICO constructor genérico (sin ambigüedad)
    public Repository(InvensaDbContext context, ILogger<Repository<T>> logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Entities = Context.Set<T>();
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
        => await Entities.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        => await Entities.AddRangeAsync(entities, cancellationToken);

    public async Task<bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        try
        {
            return await Entities.AllAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred: {Message}.", ex.Message);
            throw new InfrastructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        try
        {
            return await Entities.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred: {Message}.", ex.Message);
            throw new InfrastructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken,
        bool? asNoTracking = null)
    {
        try
        {
            return asNoTracking is null or false
                ? await Entities.FirstOrDefaultAsync(predicate, cancellationToken)
                : await Entities.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred: {Message}.", ex.Message);
            throw new InfrastructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool? asNoTracking = null)
    {
        try
        {
            return asNoTracking is null or false
                ? await Entities.ToListAsync(cancellationToken)
                : await Entities.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred: {Message}.", ex.Message);
            throw new InfrastructureException(DefaultDatabaseErrorMessage);
        }
    }

    public void Remove(T entity) => Entities.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => Entities.RemoveRange(entities);

    public void Update(T entity) => Entities.Update(entity);

    public void UpdateRange(IEnumerable<T> entities) => Entities.UpdateRange(entities);

    public async Task<IEnumerable<T>> WhereAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken,
        bool? asNoTracking = null)
    {
        try
        {
            return asNoTracking is null or false
                ? await Entities.Where(predicate).ToListAsync(cancellationToken)
                : await Entities.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception has occurred: {Message}.", ex.Message);
            throw new InfrastructureException(DefaultDatabaseErrorMessage);
        }
    }

    public async Task<T?> FindAsync(CancellationToken cancellationToken, params object[] id)
        => await Entities.FindAsync(id, cancellationToken);

    public IQueryable<T> AsQueryable() => Entities;
}
