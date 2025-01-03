namespace Application.UseCases.CheckPulse.Infrastructure;

public interface ICheckPulseRepository
{
    Task<string[]> RetrieveVitalReadings(CancellationToken cancellationToken = default);

    Task SaveNewVitalCheck();
}