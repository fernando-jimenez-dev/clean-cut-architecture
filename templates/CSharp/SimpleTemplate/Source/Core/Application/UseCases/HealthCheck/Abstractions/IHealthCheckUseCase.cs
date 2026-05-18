using Application.Shared.Abstractions.UseCase;
using Application.UseCases.HealthCheck.Errors;

namespace Application.UseCases.HealthCheck.Abstractions;

public interface IHealthCheckUseCase : IUseCase<HealthCheckError>
{
}
