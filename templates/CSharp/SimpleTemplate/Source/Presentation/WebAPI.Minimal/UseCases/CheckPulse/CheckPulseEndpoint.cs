using Application.UseCases.CheckPulse.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Minimal.UseCases.CheckPulse;

public class CheckPulseEndpoint
{
    public static async Task<IResult> Execute(
        [FromServices] ICheckPulseUseCase checkPulseUseCase,
        [FromServices] ILogger<CheckPulseEndpoint> logger,
        CancellationToken cancellationToken
        )
    {
        try
        {
            var input = "Default use case input.";
            var output = await checkPulseUseCase.Run(input, cancellationToken);

            if (output.IsSuccess)
            {
                logger.LogInformation("Pulse checked!");
                return Results.Json(
                    data: new CheckPulseEndpointResponse(Message: "Pulse checked!"),
                    statusCode: (int)HttpStatusCode.OK
                );
            }

            logger.LogError("Pulse check failed with message {}", output.ErrorMessage);
            return Results.Json(
                data: new CheckPulseEndpointResponse(Message: output.ErrorMessage ?? string.Empty),
                statusCode: (int)HttpStatusCode.InternalServerError
            );
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "There was an unrecoverable error while pulse checking.");
            return Results.Json(
                data: new CheckPulseEndpointResponse(Message: "Unrecoverable error encountered."),
                statusCode: (int)HttpStatusCode.InternalServerError
            );
        }
    }
}