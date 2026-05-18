using Application.UseCases.HealthCheck.Abstractions;
using Application.UseCases.HealthCheck.Errors;
using Shared.ResultPattern;

namespace Application.UseCases.HealthCheck;

/// <summary>
/// A simple use case to verify the application's operational state.
/// Use this class as a reference to start implementing your very own use cases.
/// </summary>
public class HealthCheckUseCase : IHealthCheckUseCase
{
    public async Task<Result<Unit, HealthCheckError>> Run(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return Result.Ok();
    }
}