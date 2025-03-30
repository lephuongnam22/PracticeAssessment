using Microsoft.EntityFrameworkCore;
using PraceticeAssesment.Entity.Repositories;

namespace PraceticeAssesment.Entity;

public interface IUnitOfWork<out TContext> where TContext : DbContext, IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
