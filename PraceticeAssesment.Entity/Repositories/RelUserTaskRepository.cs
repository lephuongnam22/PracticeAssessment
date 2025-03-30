using PraceticeAssesment.Entity.Models;

namespace PraceticeAssesment.Entity.Repositories;

public interface IRelUserTaskRepository : IRepository<RelUserTaskEntity>
{
}

public class RelUserTaskRepository(DatabaseContext dbContext)
    : Repository<RelUserTaskEntity>(dbContext), IRelUserTaskRepository
{

}
