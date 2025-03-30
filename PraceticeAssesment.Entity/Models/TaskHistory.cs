namespace PraceticeAssesment.Entity.Models;

public class TaskHistory
{
    public int Id { get; set; }

    public virtual TaskEntity Task { get; set; } = null!;

    public int TaskId { get; set; }

    public int OldStatus { get; set; }

    public int NewStatus { get; set; }

    public DateTime ChangedDate { get; set; }

    public int UserId { get; set; }

}
