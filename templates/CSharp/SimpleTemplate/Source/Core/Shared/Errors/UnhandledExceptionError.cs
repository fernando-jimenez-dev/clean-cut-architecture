using Shared.ResultPattern;

namespace Shared.Errors;

/// <summary>
/// Represents an unexpected failure caused by an unhandled exception.
///
/// This error is used as a safety net when an exception is caught that does not
/// have a known, modeled error representation. It allows the application to
/// return a controlled failure result while preserving the original exception
/// for diagnostics and logging.
///
/// <para>
/// The presence of this error in logs indicates a gap in the error model and
/// should trigger investigation and the creation of a more specific error type.
/// </para>
///
/// <para>
/// Unlike other error types, <see cref="UnhandledExceptionError"/> always wraps a non-null
/// <see cref="Exception"/> instance.
/// </para>
/// </summary>

public sealed record UnhandledExceptionError : Error
{
    public UnhandledExceptionError(
        Exception exception,
        string? message = null,
        IReadOnlyList<Error>? causes = null)
        : base(
            code: "app.unhandled-exception",
            message: message ?? "An unhandled exception occurred.",
            causes: causes)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
}