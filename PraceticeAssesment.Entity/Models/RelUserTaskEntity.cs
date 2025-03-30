namespace PraceticeAssesment.Entity.Models;

public class RelUserTaskEntity
{
    public int Id { get; set; }

    public virtual UserEntity User { get; set; } = null!;

    public int UserId { get; set; }

    public virtual TaskEntity Task { get; set; } = null!;

    public int TaskId { get; set; }
}
