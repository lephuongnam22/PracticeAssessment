namespace PraceticeAssesment.Entity.Models;

public class TaskEntity : BaseEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Status { get; set; }

    public DateTime DueDate { get; set; }

    public virtual ICollection<TaskHistory> TaskHistories { get; set; } = [];

    public virtual ICollection<RelUserTaskEntity> RelUserTaskEntities { get; set; } = [];

    public virtual ICollection<TaskCommentEntity> TaskComments { get; set; } = [];

    public virtual ICollection<NotificationEntity> NotificationEntities { get; set; } = [];
}
