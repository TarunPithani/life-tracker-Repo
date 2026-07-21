using System.Net;
using System.Text.Json;
using FluentValidation;
using LifeTracker.Application.Exceptions;

namespace LifeTracker.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, body) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                CreateErrorBody(notFound.Message)),
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                CreateValidationErrorBody(validation)),
            ArgumentException argument => (
                HttpStatusCode.BadRequest,
                CreateErrorBody(argument.Message)),
            _ => (
                HttpStatusCode.InternalServerError,
                CreateErrorBody("An unexpected error occurred."))
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }

    private static object CreateErrorBody(string message) => new
    {
        error = message
    };

    private static object CreateValidationErrorBody(ValidationException exception) => new
    {
        error = "Validation failed.",
        errors = exception.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray())
    };
}
