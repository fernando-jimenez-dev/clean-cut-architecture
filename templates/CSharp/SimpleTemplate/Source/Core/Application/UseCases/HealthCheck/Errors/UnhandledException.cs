using Shared.ResultPattern;

namespace Application.UseCases.HealthCheck.Errors;

/// <summary>
/// An unhandled error typically caught by a high level try-catch.
/// </summary>
/// <param name="Exception">Unhandled exception</param>
public sealed record UnhandledException(Exception Exception, ErrorContext? Context = null) : HealthCheckError(Context);