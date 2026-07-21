using System.Linq.Expressions;
using LifeTracker.Application.Interfaces.Repositories;
using LifeTracker.Domain.Common;
using LifeTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeTracker.Infrastructure.Repositories;

public class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity>
    where TEntity : class, IEntity
{
    protected AppDbContext Context { get; } = context;
    protected DbSet<TEntity> Entities { get; } = context.Set<TEntity>();

    public virtual Task<TEntity?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return Entities
            .AsNoTracking()
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual async Task<IReadOnlyCollection<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = Entities.AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public virtual Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(filter, cancellationToken);
    }

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await Entities.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await Entities.AddRangeAsync(entities, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DetachTrackedEntity(entity.Id);
        Entities.Update(entity);

        if (entity is IAuditEntity auditEntity)
        {
            var entry = Context.Entry(auditEntity);
            entry.Property(item => item.CreatedBy).IsModified = false;
            entry.Property(item => item.CreatedOn).IsModified = false;
            entry.Property(item => item.Version).IsModified = false;

            if (auditEntity.Version is not null)
            {
                entry.Property(item => item.Version).OriginalValue = auditEntity.Version;
            }
        }

        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        Entities.Remove(entity);
        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Entities.FindAsync([id], cancellationToken);

        if (entity is not null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public virtual Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }

    private void DetachTrackedEntity(long id)
    {
        var trackedEntity = Entities.Local.FirstOrDefault(entity => entity.Id == id);

        if (trackedEntity is not null)
        {
            Context.Entry(trackedEntity).State = EntityState.Detached;
        }
    }
}
