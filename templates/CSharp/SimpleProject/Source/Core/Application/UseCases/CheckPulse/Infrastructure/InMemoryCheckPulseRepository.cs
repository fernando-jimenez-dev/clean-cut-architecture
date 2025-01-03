using FluentResults;

namespace Application.UseCases.CheckPulse.Infrastructure;

public class InMemoryCheckPulseRepository : ICheckPulseRepository
{
    public Task<Result<string[]>> RetrieveVitalReadings(CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Ok(new string[] { "All", "Good" }));

    public Task<Result> SaveNewVitalCheck()
        => Task.FromResult(Result.Ok());
}