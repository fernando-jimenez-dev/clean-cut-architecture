namespace Shared.ResultPattern;

/// <summary>
/// Represents the absence of a meaningful value — the unit type for void operations.
///
/// <para>
/// Used as the <c>TValue</c> type parameter in <see cref="Result{TValue, TError}"/> when an
/// operation succeeds with no data. This collapses the two-type Result family into one:
/// <c>Result&lt;Unit, TError&gt;</c> for void operations, <c>Result&lt;TData, TError&gt;</c> for data operations.
/// </para>
///
/// <para>
/// Use <c>Result.Ok&lt;TError&gt;()</c> to create a unit success.
/// </para>
/// </summary>
public readonly record struct Unit;