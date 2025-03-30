using PraceticeAssesment.Entity.Models;

namespace PraceticeAssesment.Entity.Repositories;

public interface INotificationRepository : IRepository<NotificationEntity>
{
}

public class NotificationRepository(DatabaseContext dbContext)
    : Repository<NotificationEntity>(dbContext), INotificationRepository
{

}