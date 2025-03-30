namespace PraceticeAssesment.Entity.Models;

public class NotificationEntity
{
    public int Id { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public virtual UserEntity User { get; set; } = null!;

    public int UserId { get; set; }

    public virtual TaskEntity Task { get; set; } = null!;

    public int TaskId { get; set; }

    public bool HasNotified { get; set; }
}
