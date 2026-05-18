using Shared.ResultPattern;

namespace Application.UseCases.HealthCheck.Errors;

/// <summary>
/// Error contract for the HealthCheck Use Case.
/// Defines all the ways the health check can fail.
/// </summary>
public abstract record HealthCheckError(ErrorContext? Context = null) : IContextualError;