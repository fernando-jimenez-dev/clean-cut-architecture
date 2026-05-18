using Shared.ResultPattern;

namespace Application.Shared.Abstractions.UseCase
{
    /// <summary>
    /// A use case with no input and no output — just success or typed failure.
    /// </summary>
    /// <typeparam name="TError">The Use Case's sealed error hierarchy (extends <see cref="UseCaseError"/>).</typeparam>
    public interface IUseCase<TError>
        where TError : class, IContextualError
    {
        Task<Result<Unit, TError>> Run(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A use case that takes input and returns output.
    /// </summary>
    /// <typeparam name="TUseCaseInput">What goes in.</typeparam>
    /// <typeparam name="TUseCaseOutput">What comes out.</typeparam>
    /// <typeparam name="TError">How it can fail.</typeparam>
    public interface IUseCase<in TUseCaseInput, TUseCaseOutput, TError>
        where TError : class, IContextualError
    {
        Task<Result<TUseCaseOutput, TError>> Run(TUseCaseInput input, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A use case that takes input but returns no output — just success or typed failure.
    /// </summary>
    /// <typeparam name="TUseCaseInput">What goes in.</typeparam>
    /// <typeparam name="TError">How it can fail.</typeparam>
    public interface IUseCase<in TUseCaseInput, TError>
        where TError : class, IContextualError
    {
        Task<Result<Unit, TError>> Run(TUseCaseInput input, CancellationToken cancellationToken = default);
    }
}

namespace Application.Shared.Abstractions.UseCase.OutputOnly
{
    /// <summary>
    /// A use case with no input that returns output.
    /// </summary>
    /// <typeparam name="TUseCaseOutput">What comes out.</typeparam>
    /// <typeparam name="TError">How it can fail.</typeparam>
    public interface IUseCase<TUseCaseOutput, TError>
        where TError : class, IContextualError
    {
        Task<Result<TUseCaseOutput, TError>> Run(CancellationToken cancellationToken = default);
    }
}
