using System.Net;

namespace PracticeAssessment.Commons;

public class BaseOperationResult
{
    public bool Succeeded => (int)StatusCode >= 200 && (int)StatusCode < 299;

    public HttpStatusCode StatusCode { get; set; }

    public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
}
