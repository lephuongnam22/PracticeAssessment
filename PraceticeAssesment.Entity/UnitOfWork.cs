using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PraceticeAssesment.Entity.Repositories;

namespace PraceticeAssesment.Entity;

public class UnitOfWork<TContext>(TContext context, IServiceProvider serviceProvider) : IUnitOfWork<TContext>, IDisposable
    where TContext : DbContext, IDisposable
{
    private bool _disposed = false;
    private readonly TContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private IDbContextTransaction? _currentTransaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);

            throw;
        }
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
                _currentTransaction?.Dispose();
            }

            _disposed = true;
        }
    }
}
