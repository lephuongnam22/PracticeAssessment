using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PraceticeAssesment.Entity.Repositories;

public class Repository<TEntity>(DbContext dbContext) : IRepository<TEntity>
    where TEntity : class
{
    private DbSet<TEntity> DbSet => dbContext.Set<TEntity>();

    public void Delete(TEntity entity)
    {
        var entry = dbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        DbSet.Remove(entity);

    }

    public IQueryable<TEntity> GetQueryable()
    {
        return GetQueryable(false);
    }

    public void Insert(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void Update(TEntity entity)
    {
        var entry = dbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        entry.State = EntityState.Modified;
    }

    private IQueryable<TEntity> GetQueryable(bool isEnabledTracking)
    {
        return isEnabledTracking ? DbSet : DbSet.AsNoTracking();
    }

    public async Task<TEntity?> GetById(int id)
    {
        return await DbSet
           .FindAsync(id);
    }

    public async Task<IList<TEntity>?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable().Where(predicate).ToListAsync(cancellationToken: cancellationToken);
    }
}
