namespace PracticeAssessment.Core.Models;

public class PagingResponse<T> where T:class
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public bool HasMore { get; set; } = false;

    public IEnumerable<T> Data { get; set; } = [];
}
