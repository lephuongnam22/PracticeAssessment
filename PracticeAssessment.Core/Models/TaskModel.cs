using System.ComponentModel.DataAnnotations;

namespace PracticeAssessment.Core.Models;

public class TaskModel
{
    public int Id { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public int Status { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public IEnumerable<int> AssignUserIds { get; set; } = [];
}