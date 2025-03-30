namespace PraceticeAssesment.Entity.Models;

public class TaskCommentEntity
{
    public int Id { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CommentDate { get; set; }

    public int UserId { get; set;}

    public virtual UserEntity User { get; set; } = null!;

    public virtual TaskEntity Task { get; set; } = null!;

    public int TaskId { get; set; }
}
