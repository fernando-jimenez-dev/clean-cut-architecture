using Shared.ResultPattern;

namespace Application.UseCases.HealthCheck.Errors;

/// <summary>
/// A required service or dependency is unreachable.
/// </summary>
/// <param name="Service">Which service is unreachable (e.g., "Database", "Cache", "PaymentGateway").</param>
public sealed record ServiceUnreachable(string Service, ErrorContext? Context = null) : HealthCheckError(Context);
