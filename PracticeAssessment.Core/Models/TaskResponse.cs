namespace PracticeAssessment.Core.Models;

public class TaskResponse : TaskModel
{
    public IEnumerable<CommentModel> Comments { get; set; } = [];

    public IEnumerable<NotificationModel> Notifications { get; set; } = [];

    public IEnumerable<TaskHistoryModel> TaskHistoryModels { get; set; } = [];
}
