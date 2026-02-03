using FluentValidation.Results;
using Shared.ResultPattern;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Shared.Errors;

/// <summary>
/// Base validation error category.
/// Endpoints can pattern-match on this type without knowing any generic arguments.
/// </summary>
public abstract record ValidationError : Error
{
    /// <summary>
    /// Human-readable validation issues (safe to return to callers when appropriate).
    /// </summary>
    public abstract IReadOnlyList<string> Issues { get; }

    protected ValidationError(string code, string message = "", IReadOnlyList<Error>? causes = null)
        : base(code, message, causes) { }
}

/// <summary>
/// Generic validation error that exposes the value that failed validation,
/// as well as the validation issues that were found.
/// </summary>
/// <remarks>
/// The error <see cref="Error.Code"/> is frozen and does not depend on <typeparamref name="TValue"/>.
/// Pattern matching should be done against <see cref="ValidationError"/> (non-generic).
/// </remarks>
/// <typeparam name="TValue">C# type that failed validation.</typeparam>
public sealed record ValidationError<TValue> : ValidationError
{
    /// <summary>
    /// Frozen code for all validation failures.
    /// </summary>
    public const string CodeValue = "validation.failed";

    /// <summary>
    /// The value that failed validation (diagnostic-only).
    /// </summary>
    [JsonIgnore]
    [IgnoreDataMember]
    public TValue Value { get; }

    /// <summary>
    /// Human-readable validation issues.
    /// </summary>
    public override IReadOnlyList<string> Issues { get; }

    /// <summary>
    /// Primary constructor: accepts any issue list.
    /// </summary>
    public ValidationError
        (TValue value, IEnumerable<string> issues, string? message = null)
        : base(
            CodeValue,
            message ?? $"Value of type {typeof(TValue).Name} failed validation.")
    {
        Value = value;
        Issues = issues?.ToList() ?? [];
    }

    /// <summary>
    /// Convenience constructor for a single issue.
    /// </summary>
    public ValidationError(TValue value, string issue, string? message = null)
        : this(value, [issue], message) { }

    /// <summary>
    /// Convenience constructor for <see cref="ValidationResult"/>.
    /// </summary>
    public ValidationError(TValue value, ValidationResult validationResult, string? message = null)
        : this(value, ExtractIssues(validationResult), message) { }

    private static IEnumerable<string> ExtractIssues(ValidationResult validationResult)
        => validationResult?.Errors?.Select(e => e.ErrorMessage) ?? [];
}

/// <summary>
/// Sugar syntax helpers for creating validation errors.
/// </summary>
public static class ValidationErrors
{
    public static ValidationError<TInput> For<TInput>(TInput input, IEnumerable<string> issues, string? message = null)
        => new(input, issues, message);

    public static ValidationError<TInput> For<TInput>(TInput input, string issue, string? message = null)
        => new(input, issue, message);

    public static ValidationError<TInput> For<TInput>(TInput input, ValidationResult validationResult, string? message = null)
        => new(input, validationResult, message);
}