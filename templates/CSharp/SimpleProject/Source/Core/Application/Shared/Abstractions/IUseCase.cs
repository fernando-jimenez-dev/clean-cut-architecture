using FluentResults;

namespace Application.Shared.Abstractions.UseCase;

/// <summary>
/// Represents a use case that performs an action and returns a <see cref="Result"/>.
/// </summary>
public interface IUseCase
{
    /// <summary>
    /// Executes the use case.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution <see cref="Result"/>.</returns>
    Task<Result> Run(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a use case that takes an input, performs an action, and returns a <see cref="Result"/> containing an output.
/// </summary>
/// <typeparam name="TUseCaseInput">The type of the use case input.</typeparam>
/// <typeparam name="TUseCaseOutput">The type of the use case output.</typeparam>
public interface IUseCase<in TUseCaseInput, TUseCaseOutput>
{
    /// <summary>
    /// Executes the use case.
    /// </summary>
    /// <param name="input">The input for the use case.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution <see cref="Result"/> and the output.</returns>
    Task<Result<TUseCaseOutput>> Run(TUseCaseInput input, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a use case that takes an input, performs an action, and returns a <see cref="Result"/>.
/// </summary>
/// <typeparam name="TUseCaseInput">The type of the use case input.</typeparam>
public interface IUseCase<in TUseCaseInput>
{
    /// <summary>
    /// Executes the use case.
    /// </summary>
    /// <param name="input">The input for the use case.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution <see cref="Result"/>.</returns>
    Task<Result> Run(TUseCaseInput input, CancellationToken cancellationToken = default);
}