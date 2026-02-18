using System.Net;
using FluentValidation;
using MedicineFinder.Application.Exceptions;

namespace MedicineFinder.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Handled application exception");
            await WriteErrorAsync(context, ex.StatusCode, ex.Code, ex.Message, ex.Details);
        }
        catch (ValidationException ex)
        {
            var details = ex.Errors.Select(e => e.ErrorMessage).ToList();
            await WriteErrorAsync(context, (int)HttpStatusCode.BadRequest, "ValidationError", "Validation failed", details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, (int)HttpStatusCode.InternalServerError, "ServerError", "An unexpected error occurred", Array.Empty<string>());
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message, IReadOnlyList<string> details)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Code = code,
            Message = message,
            Details = details
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public IReadOnlyList<string> Details { get; set; } = Array.Empty<string>();
}
