using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace PraceticeAssesment.Entity.Repositories;

public interface IRepository<TEntity> where TEntity: class
{
    IQueryable<TEntity> GetQueryable();

    void Insert(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task<TEntity?> GetById(int id);

    Task<IList<TEntity>?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
