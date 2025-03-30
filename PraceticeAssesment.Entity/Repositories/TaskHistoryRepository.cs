using PraceticeAssesment.Entity.Models;

namespace PraceticeAssesment.Entity.Repositories;

public interface ITaskHistoryRepository : IRepository<TaskHistory>
{ 
}

public class TaskHistoryRepository(DatabaseContext dbContext)
    : Repository<TaskHistory>(dbContext), ITaskHistoryRepository
{

}