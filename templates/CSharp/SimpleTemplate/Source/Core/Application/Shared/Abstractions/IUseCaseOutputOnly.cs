namespace Application.Shared.Abstractions.UseCase.OutputOnly;

/// <summary>
/// Represents a use case that performs an action and returns an output.
/// </summary>
/// <typeparam name="TUseCaseOutput">The type of the use case output.</typeparam>
public interface IUseCase<TUseCaseOutput>
{
    /// <summary>
    /// Executes the use case.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution output.</returns>
    Task<TUseCaseOutput> Run(CancellationToken cancellationToken = default);
}