using Microsoft.AspNetCore.Mvc;

namespace InsuranceApi.Errors;

public static class ProblemFactory
{
    public static IActionResult BadRequest(string code, string message)
    {
        return new ObjectResult(new ProblemDetails
        {
            Title = "Bad request",
            Detail = message,
            Status = StatusCodes.Status400BadRequest,
            Extensions = { ["code"] = code }
        })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult NotFound(string code, string message)
    {
        return new ObjectResult(new ProblemDetails
        {
            Title = "Not found",
            Detail = message,
            Status = StatusCodes.Status404NotFound,
            Extensions = { ["code"] = code }
        })
        {
            StatusCode = StatusCodes.Status404NotFound
        };
    }

    public static IActionResult Conflict(string code, string message)
    {
        return new ObjectResult(new ProblemDetails
        {
            Title = "Conflict",
            Detail = message,
            Status = StatusCodes.Status409Conflict,
            Extensions = { ["code"] = code }
        })
        {
            StatusCode = StatusCodes.Status409Conflict
        };
    }
}