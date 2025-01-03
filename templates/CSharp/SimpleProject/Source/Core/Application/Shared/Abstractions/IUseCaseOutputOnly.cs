using FluentResults;

namespace Application.Shared.Abstractions.UseCase.OutputOnly;

/// <summary>
/// Represents a use case that performs an action and returns a <see cref="Result"/> containing an output.
/// </summary>
/// <typeparam name="TUseCaseOutput">The type of the use case output.</typeparam>
public interface IUseCase<TUseCaseOutput>
{
    /// <summary>
    /// Executes the use case.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution <see cref="Result{TUseCaseOutput}"/> and the output.</returns>
    Task<Result<TUseCaseOutput>> Run(CancellationToken cancellationToken = default);
}