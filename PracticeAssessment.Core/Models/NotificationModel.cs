namespace PracticeAssessment.Core.Models;

public class NotificationModel
{
    public int Id { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int TaskId { get; set; }

    public bool HasNotified { get; set; }

    public int UserId { get; set; }
}
