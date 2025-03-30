using PraceticeAssesment.Entity.Models;

namespace PraceticeAssesment.Entity.Repositories;

public interface IUserRepository : IRepository<UserEntity>
{
}

public class UserRepository(DatabaseContext dbContext)
    : Repository<UserEntity>(dbContext), IUserRepository
{

}
