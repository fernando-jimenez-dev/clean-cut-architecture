namespace Shared.ResultPattern;

/// <summary>
/// Opt-in contract for errors that carry diagnostic context.
///
/// <para>
/// Implementing this interface signals that an error can carry an optional
/// <see cref="ErrorContext"/> — the "why did it fail?" diagnostic layer.
/// Context is set at construction time and is immutable after that.
/// </para>
///
/// <para>
/// The IUseCase interfaces constrain their TError to this interface, so every Use Case
/// error hierarchy is guaranteed to be able to carry context. Components outside of
/// Use Cases (repositories, HTTP clients, utilities) use Result freely without this interface.
/// </para>
///
/// <para>
/// Not every error needs context — simple, self-explanatory errors (e.g., <c>OrderNotFound</c>)
/// can skip this interface entirely. Rich errors that wrap infrastructure failures or encode
/// diagnostic information should implement it.
/// </para>
///
/// <example>
/// Simple error — no context needed:
/// <code>
/// public sealed record OrderNotFound(string OrderId);
/// </code>
///
/// Rich error — opts into context:
/// <code>
/// public sealed record ServiceUnavailable(string Service, ErrorContext? Context = null)
///     : IContextualError;
/// </code>
///
/// Per-Use-Case sealed error family:
/// <code>
/// public abstract record CreateUserError(ErrorContext? Context = null) : IContextualError;
/// public sealed record EmailAlreadyExists(string Email, ErrorContext? Context = null) : CreateUserError(Context);
/// public sealed record ServiceUnavailable(ErrorContext? Context = null) : CreateUserError(Context);
/// </code>
///
/// Logging middleware reads Context generically:
/// <code>
/// if (result.Failed(out var error))
///     logger.LogError(error.Context?.Message);
/// </code>
/// </example>
/// </summary>
public interface IContextualError
{
    /// <summary>
    /// Optional diagnostic context. The "why did it fail?" layer.
    /// Consumed by logging and observability. Never by Presentation. Never for control flow.
    /// </summary>
    ErrorContext? Context { get; }
}