using System.Diagnostics.CodeAnalysis;

namespace Shared.ResultPattern;

/// <summary>
/// Represents the outcome of an operation — either success with a value, or failure with a typed error.
///
/// <para>
/// Result is a generic container — a box. It has no opinion about what <typeparamref name="TError"/> is.
/// The only constraint is <c>class</c>, needed so <c>null</c> can represent "no error" (success).
/// </para>
///
/// <para>
/// Consumers of Result decide whether to add further constraints. For example, the IUseCase
/// interfaces require <c>TError : IContextualError</c>, guaranteeing that all Use Case errors
/// can carry optional <see cref="ErrorContext"/>. But a shared repository, HTTP client wrapper,
/// or utility can use Result freely with any error class — no interface required.
/// </para>
///
/// <para>
/// For void operations (no data on success), use <c>Result&lt;Unit, TError&gt;</c> and return
/// <c>Result.Ok</c> — the implicit conversion from <see cref="Unit"/> handles the wrapping.
/// </para>
///
/// <example>
/// Unit operation (no output):
/// <code>
/// public Task&lt;Result&lt;Unit, DeleteUserError&gt;&gt; Run(Guid userId, CancellationToken ct)
/// {
///     // ...
///     return Task.FromResult&lt;Result&lt;Unit, DeleteUserError&gt;&gt;(Result.Ok);
/// }
/// </code>
///
/// Data operation:
/// <code>
/// public Task&lt;Result&lt;CreatedUser, CreateUserError&gt;&gt; Run(CreateUserInput input, CancellationToken ct)
/// {
///     // ...
///     return createdUser; // implicit conversion
/// }
/// </code>
/// </example>
/// </summary>
/// <typeparam name="TValue">
/// The type returned on success. Use <see cref="Unit"/> for operations with no output.
/// </typeparam>
/// <typeparam name="TError">
/// Any reference type representing failure. The box doesn't care what it is —
/// the constraint lives on whoever uses the box (e.g., IUseCase).
/// </typeparam>
public record Result<TValue, TError> where TError : class
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    /// <summary>The value on success, <c>default</c> on failure.</summary>
    public TValue? Value { get; }

    /// <summary>The error on failure, <c>null</c> on success.</summary>
    public TError? Error { get; }

    private Result(TValue? value, TError? error)
    {
        IsSuccess = error is null;
        Value = value;
        Error = error;
    }

    // --- Consumption ---

    /// <summary>Returns <c>true</c> if the operation succeeded.</summary>
    public bool Succeeded() => IsSuccess;

    /// <summary>Returns <c>true</c> if succeeded, outputting the value.</summary>
    public bool Succeeded([MaybeNullWhen(false)] out TValue? value)
    {
        value = Value;
        return IsSuccess;
    }

    /// <summary>Returns <c>true</c> if the operation failed.</summary>
    public bool Failed() => IsFailure;

    /// <summary>Returns <c>true</c> if failed, outputting the error for pattern matching.</summary>
    public bool Failed([NotNullWhen(true)] out TError? error)
    {
        error = Error;
        return IsFailure;
    }

    // --- Functional ---

    /// <summary>
    /// Applies one of two functions depending on the outcome, returning a unified result.
    /// Forces exhaustive handling of both branches inline.
    /// </summary>
    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<TError, TResult> failure)
        => IsSuccess ? success(Value!) : failure(Error!);

    /// <summary>Unit variant of <see cref="Match{TResult}"/> — side-effects only.</summary>
    public void Match(Action<TValue> success, Action<TError> failure)
    {
        if (IsSuccess) success(Value!);
        else failure(Error!);
    }

    /// <summary>
    /// Transforms the success value without unwrapping.
    /// A failed Result passes the error through unchanged.
    /// </summary>
    public Result<TNew, TError> Map<TNew>(Func<TValue, TNew> transform)
        => IsSuccess ? Result<TNew, TError>.Success(transform(Value!)) : Error!;

    /// <summary>
    /// Chains an operation that itself returns a Result.
    /// A failed Result short-circuits — <paramref name="next"/> is never called.
    /// </summary>
    public Result<TNew, TError> Then<TNew>(Func<TValue, Result<TNew, TError>> next)
        => IsSuccess ? next(Value!) : Error!;

    // --- Factory ---

    public static Result<TValue, TError> Success(TValue value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        return new(value, default);
    }

    public static Result<TValue, TError> Failure(TError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(default, error);
    }

    // --- Implicit conversions ---

    /// <summary>
    /// Return a value directly — implicit conversion wraps it in a successful Result.
    /// <code>return createdUser;</code>
    /// </summary>
    public static implicit operator Result<TValue, TError>(TValue value) => Success(value);

    /// <summary>
    /// Return an error directly — implicit conversion wraps it in a failed Result.
    /// <code>return new EmailAlreadyExists(email);</code>
    /// </summary>
    public static implicit operator Result<TValue, TError>(TError error) => Failure(error);
}

/// <summary>
/// Static factory shorthand for creating Result instances.
///
/// <para>
/// <c>Result.Ok</c> is the void success sentinel — return it directly from methods that return
/// <c>Result&lt;Unit, TError&gt;</c> and the implicit conversion handles the wrapping.
/// </para>
///
/// <para>
/// <c>Result.Ok&lt;TError&gt;()</c> and <c>Result.Ok&lt;TData, TError&gt;(data)</c> are explicit
/// factory alternatives when the implicit conversion can't infer the types (e.g., in test setup).
/// </para>
/// </summary>
public static class Result
{
    /// <summary>
    /// Unit success shorthand — returns <see cref="Unit"/> directly.
    /// In an async method, the implicit conversion from <c>Unit</c> wraps it in a <c>Result&lt;Unit, TError&gt;</c> automatically.
    /// <code>
    /// public async Task&lt;Result&lt;Unit, DeleteUserError&gt;&gt; Run(...) { ...; return Result.Ok(); }
    /// </code>
    /// </summary>
    public static Unit Ok() => default;

    /// <summary>
    /// Unit success factory with explicit error type — for non-async returns, test setup, and mock returns
    /// where the implicit conversion can't infer the result type.
    /// </summary>
    public static Result<Unit, TError> Ok<TError>() where TError : class
        => Result<Unit, TError>.Success(default);

    /// <summary>Data success factory.</summary>
    public static Result<TData, TError> Ok<TData, TError>(TData data) where TError : class
        => Result<TData, TError>.Success(data);

    /// <summary>Explicit void failure factory.</summary>
    public static Result<Unit, TError> Fail<TError>(TError error) where TError : class
        => Result<Unit, TError>.Failure(error);

    /// <summary>Explicit data-typed failure factory — for when the full signature is needed but only an error exists.</summary>
    public static Result<TData, TError> Fail<TData, TError>(TError error) where TError : class
        => Result<TData, TError>.Failure(error);
}