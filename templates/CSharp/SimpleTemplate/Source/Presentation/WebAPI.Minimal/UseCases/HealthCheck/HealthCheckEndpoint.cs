using Application.UseCases.HealthCheck.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Shared.Errors;
using Shared.ResultPattern;
using System.Net;

namespace WebAPI.Minimal.UseCases.HealthCheck;

public class HealthCheckEndpoint
{
    public static async Task<IResult> Execute(
        [FromServices] IHealthCheckUseCase _healthCheckUseCase,
        [FromServices] ILogger<HealthCheckEndpoint> _logger,
        CancellationToken _cancellationToken
    )
    {
        var result = await _healthCheckUseCase.Run(_cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogTrace("Healthy!");
            return CreateJsonResponse(
                new HealthCheckEndpointResponse("Healthy!"), HttpStatusCode.OK);
        }

        return HandleFailure(result.Error!, _logger);
    }

    private static IResult HandleFailure(Error error, ILogger logger)
    {
        // UnexpectedError is a "patch me" signal: log it loudly.
        if (error is UnhandledExceptionError unexpectedError)
        {
            logger.LogError(
                unexpectedError.Exception,
                "HealthCheck failed with an unhandled exception. Code: {Code}",
                unexpectedError.Code
            );
            return CreateErrorResponse("Unhealthy.", HttpStatusCode.InternalServerError);
        }

        // If the use case ever returns a modeled error, map it here.
        // -----

        // Fallback for unmapped errors.
        logger.LogError(
            "HealthCheck failed with an error not mapped by endpoint. Code: {Code}. Message: {Message}",
            error.Code,
            error.Message
        );
        return CreateErrorResponse("Unhealthy.", HttpStatusCode.InternalServerError);
    }

    private static IResult CreateJsonResponse(HealthCheckEndpointResponse response, HttpStatusCode statusCode)
        => Results.Json(data: response, statusCode: (int)statusCode);

    private static IResult CreateErrorResponse(string message, HttpStatusCode statusCode)
        => CreateJsonResponse(new HealthCheckEndpointResponse(message), statusCode);
}