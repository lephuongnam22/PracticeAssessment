namespace PracticeAssessment.Services;

public interface INotificationService
{
    Task NotificationToUser(int taskId);
}

public class NotificationService : INotificationService
{
    public Task NotificationToUser(int taskIds)
    {
        return Task.CompletedTask;
    }
}
