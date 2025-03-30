using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PracticeAssessment.Commons;

public static class OperationResultExtensions
{
    public static IResult ToResult(this OperationResult operationResult)
    {
        if (operationResult.StatusCode == HttpStatusCode.BadRequest)
        {
            return Results.BadRequest(operationResult.Errors);
        }

        return Results.StatusCode((int)operationResult.StatusCode);
    }

    public static IResult ToResult<T>(this OperationResult<T> operationResult)
    {
        return operationResult.StatusCode switch
        {
            HttpStatusCode.OK => Results.Ok(operationResult.Value),
            HttpStatusCode.BadRequest => Results.BadRequest(operationResult.Errors),
            _ => Results.StatusCode((int)operationResult.StatusCode),
        };
    }

    public static ActionResult ToActionResult(this OperationResult operationResult)
    {
        if (operationResult.StatusCode == HttpStatusCode.BadRequest)
        {
            return new BadRequestObjectResult(operationResult.Errors);
        }

        return new StatusCodeResult((int)operationResult.StatusCode);
    }

    public static ActionResult ToActionResult<T>(this OperationResult<T> operationResult)
    {
        return operationResult.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(operationResult.Value),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(operationResult.Errors),
            _ => new StatusCodeResult((int)operationResult.StatusCode),
        };
    }
}
