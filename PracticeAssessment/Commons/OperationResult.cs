using System.Net;

namespace PracticeAssessment.Commons;

public class OperationResult: BaseOperationResult
{
    public OperationResult(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public static OperationResult Status(HttpStatusCode statusCode) => new(statusCode);

    public static OperationResult Ok() => new(HttpStatusCode.OK);

    public static OperationResult NoContent() => new(HttpStatusCode.NoContent);

    public static OperationResult NotFound() => new(HttpStatusCode.NotFound);

    public static OperationResult Forbidden() => new(HttpStatusCode.Forbidden);

    public static OperationResult BadRequest(string errorMessage)
        => new(HttpStatusCode.BadRequest) { Errors = { { "message", errorMessage } } };

    public static OperationResult BadRequest(string errorCode, string errorMessage)
        => new(HttpStatusCode.BadRequest) { Errors = { { errorCode, errorMessage } } };

    public static OperationResult BadRequest(IDictionary<string, string> errors)
        => new(HttpStatusCode.BadRequest) { Errors = errors };

    public static OperationResult InternalServerError(string errorMessage)
        => new(HttpStatusCode.InternalServerError) { Errors = { { "message", errorMessage } } };

    public static OperationResult<T> Ok<T>(T value) => new(HttpStatusCode.OK, value);
}

public sealed class OperationResult<T> : BaseOperationResult
{
    public OperationResult(HttpStatusCode statusCode, T? value)
    {
        StatusCode = statusCode;
        Value = value;
    }

    public OperationResult(OperationResult result, T? value = default)
    {
        StatusCode = result.StatusCode;
        Errors = result.Errors;
        Value = value;
    }

    public T? Value { get; set; }

    public static implicit operator OperationResult<T>(OperationResult result)
    {
        return new OperationResult<T>(result);
    }

    public static implicit operator OperationResult<T>(T value)
    {
        return OperationResult.Ok(value);
    }
}
