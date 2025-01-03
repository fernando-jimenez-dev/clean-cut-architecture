using FluentResults;

namespace Application.UseCases.CheckPulse.Infrastructure;

public interface ICheckPulseRepository
{
    Task<Result<string[]>> RetrieveVitalReadings(CancellationToken cancellationToken = default);

    Task<Result> SaveNewVitalCheck();
}