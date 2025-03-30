using System.ComponentModel.DataAnnotations;

namespace PracticeAssessment.Core.Models;

public class CommentModel
{
    public int Id { get; set; }

    [Required]
    public required string Comment { get; set; }

    public int UserId { get; set; }

    public int TaskId { get; set; }

    public DateTime CreatedAt { get; set; }
}
