using PraceticeAssesment.Entity.Models;

namespace PraceticeAssesment.Entity.Repositories;


public interface ITaskRepository : IRepository<TaskEntity>
{
}

public class TaskRepository(DatabaseContext dbContext)
    : Repository<TaskEntity>(dbContext), ITaskRepository
{

}
