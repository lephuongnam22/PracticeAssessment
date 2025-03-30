namespace PracticeAssessment.Core.Models;

public class TaskHistoryModel
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int OldStatus { get; set; }

    public int NewStatus { get; set; }

    public DateTime ChangedDate { get; set; }
}
