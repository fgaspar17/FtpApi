using FtpApi.Application.DTOs;
using FtpApi.Application.Exceptions;
using FtpApi.Application.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FtpApi.Endpoints;

public static class ErrorEndpoint
{
    public static void MapErrorEndpoint(this WebApplication app)
    {
        app.MapGet("/error", (HttpContext context) =>
        {
            using var scope = app.Services.CreateScope();

            var exceptionHandler =
            context.Features.Get<IExceptionHandlerPathFeature>();

            var logger = scope.ServiceProvider.GetService<ILogger>();
            logger?.LogError(exceptionHandler?.Error, "An unhandled exception occurred.");

            var factory = new ProblemDetailsFactory();
            var details = factory.ProblemDetailsFactoryMethod(exceptionHandler, context);
            
            return Results.Problem(details);
        });
    }
}

public class ProblemDetailsFactory
{
    public ProblemDetails ProblemDetailsFactoryMethod(IExceptionHandlerPathFeature? exceptionHandler, HttpContext context)
    {
        var details = new ProblemDetails();
        details.Detail = exceptionHandler?.Error.Message;
        details.Extensions["traceId"] =
            System.Diagnostics.Activity.Current?.Id
              ?? context.TraceIdentifier;
        details.Type =
                    "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        details.Status = StatusCodes.Status500InternalServerError;

        if (exceptionHandler?.Error.GetType() == typeof(CustomValidationException))
        {
            details.Type =
                    "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
            details.Status = StatusCodes.Status400BadRequest;
            details.Detail = string.Join("; ", ((CustomValidationException)exceptionHandler?.Error).Errors);
        }
        else if (exceptionHandler?.Error.GetType() == typeof(UserRegistrationException))
        {
            details.Type =
                    "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
            details.Status = StatusCodes.Status400BadRequest;
            details.Detail = string.Join("; ", ((UserRegistrationException)exceptionHandler?.Error).Errors);
        }

        return details;
    }
}
