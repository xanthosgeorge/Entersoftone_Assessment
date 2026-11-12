
using EmployeeManagement.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.ExceptionHandling;

/// <summary>
/// Provides centralized exception handling for the API,
/// converting application exceptions into standardized
/// HTTP Problem Details responses.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Handles exceptions thrown during HTTP request processing.
    /// </summary>
    /// <param name="httpContext">
    /// The HTTP context associated with the failed request.
    /// </param>
    /// <param name="exception">
    /// The exception that occurred during request execution.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// True if the exception was handled successfully.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ConflictException => StatusCodes.Status409Conflict,
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception,
                "An unexpected error occurred while processing the request.");
        }
        else
        {
            logger.LogWarning(
                "Request failed with status code {StatusCode}: {Message}",
                statusCode,
                exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                400 => "Bad Request",
                404 => "Resource Not Found",
                409 => "Conflict",
                _ => "Internal Server Error"
            },
            Detail = statusCode == 500
                ? "An unexpected server error occurred."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}
