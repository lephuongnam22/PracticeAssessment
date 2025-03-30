namespace PracticeAssessment.Core.Models;

public class TaskByUserRequest
{
    public int Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

}
