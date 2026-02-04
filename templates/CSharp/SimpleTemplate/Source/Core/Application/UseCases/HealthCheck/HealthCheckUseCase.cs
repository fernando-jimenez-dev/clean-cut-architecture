using Application.UseCases.HealthCheck.Abstractions;
using Shared.ResultPattern;

namespace Application.UseCases.HealthCheck;

/// <summary>
/// A simple use case to verify the application's operational state.
/// Use this class as a reference to start implementing your very own use cases.
/// </summary>
public class HealthCheckUseCase : IHealthCheckUseCase
{
    public Task<Result> Run(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }
}