using PraceticeAssesment.Entity.Models;

namespace PraceticeAssesment.Entity.Repositories;

public interface ITaskCommentRepository : IRepository<TaskCommentEntity>
{
}
public class TaskCommentRepository(DatabaseContext dbContext) 
    : Repository<TaskCommentEntity>(dbContext), ITaskCommentRepository
{
}
