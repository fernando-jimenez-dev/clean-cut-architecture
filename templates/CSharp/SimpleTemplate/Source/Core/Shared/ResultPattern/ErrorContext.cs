namespace Shared.ResultPattern;

/// <summary>
/// Diagnostic information about an error — the "why did it fail?" layer.
///
/// <para>
/// ErrorContext is a standalone tool. Any error type can carry it as an optional property.
/// It is never used for control flow or branching — Presentation never reads it.
/// Only logging and observability infrastructure consume it.
/// </para>
///
/// <para>
/// Most errors won't need Context. You add it when there's diagnostic value worth
/// capturing — infrastructure failures with inner causes, timeouts with timing data,
/// business rules that failed for non-obvious reasons.
/// </para>
///
/// <para>
/// ErrorContext exists independently of <see cref="IUseCaseError"/>. The interface
/// makes Context discoverable and required for Use Case errors. But any error type —
/// shared component errors, infrastructure errors, anything — can carry an ErrorContext
/// property without implementing any interface.
/// </para>
/// </summary>
public sealed record ErrorContext
{
    /// <summary>
    /// Human-readable description of what went wrong.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Which component or layer generated this (e.g., "UserRepository", "PaymentGateway").
    /// Helps narrow down the origin in logs without reading stack traces.
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// The inner context that caused this one, forming a causal chain.
    /// E.g., "database write failed" → "connection timed out" → "DNS resolution failed".
    /// Observability reads the full chain. Nobody else needs to.
    /// </summary>
    public ErrorContext? Inner { get; init; }

    /// <summary>
    /// The original exception, if one was caught. Diagnostic-only — never serialized,
    /// never used for control flow. Exists so logging can capture stack traces.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Structured key-value pairs for observability (correlation IDs, timing, request IDs).
    /// </summary>
    public IReadOnlyDictionary<string, object>? Metadata { get; init; }

    public ErrorContext(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates context from a caught exception. The most common entry point —
    /// infrastructure catches an exception and wraps it for diagnostics.
    /// </summary>
    public static ErrorContext FromException(Exception exception, string? source = null) => new(exception.Message)
    {
        Exception = exception,
        Source = source
    };

    /// <summary>
    /// Wraps an existing context with additional context, building the causal chain.
    /// Used when a higher-level component adds its own perspective to a lower-level failure.
    /// </summary>
    public static ErrorContext Wrap(string message, ErrorContext inner, string? source = null) => new(message)
    {
        Inner = inner,
        Source = source
    };
}