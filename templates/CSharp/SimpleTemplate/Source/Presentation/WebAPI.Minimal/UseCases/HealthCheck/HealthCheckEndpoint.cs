using Application.UseCases.HealthCheck.Abstractions;
using Application.UseCases.HealthCheck.Errors;
using Microsoft.AspNetCore.Mvc;
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

        if (result.Succeeded())
        {
            return HandleFailure(error, _logger);
        }

        _logger.LogTrace("Healthy!");
        return CreateJsonResponse(
            new HealthCheckEndpointResponse("Healthy!"), HttpStatusCode.OK);
    }

        if (result.Failed(out var error))
        {
            if (error is UnhandledException ue)
                _logger.LogError(ue.Exception, "Unhandled exception in health check");
            else
                _logger.LogError(error.Context?.Message);

            return error switch
            {
                ServiceUnreachable e => CreateErrorResponse(
                    $"Unhealthy: {e.Service} is unreachable.", HttpStatusCode.ServiceUnavailable),

                UnhandledException => CreateErrorResponse(
                    "Unhealthy: unexpected failure.", HttpStatusCode.InternalServerError),

                _ => CreateErrorResponse(
                    "Unhealthy.", HttpStatusCode.InternalServerError)
            };
        }

        return CreateErrorResponse("Unhealthy.", HttpStatusCode.InternalServerError);
    }

    private static IResult CreateJsonResponse(HealthCheckEndpointResponse response, HttpStatusCode statusCode)
        => Results.Json(data: response, statusCode: (int)statusCode);

    private static IResult CreateErrorResponse(string message, HttpStatusCode statusCode)
        => CreateJsonResponse(new HealthCheckEndpointResponse(message), statusCode);
}
