using Application.UseCases.CheckPulse.Infrastructure;

namespace Application.UnitTests.UseCases.CheckPulse.Infrastructure;

public class InMemoryCheckPulseRepositoryTests
{
    private readonly InMemoryCheckPulseRepository repository;

    public InMemoryCheckPulseRepositoryTests()
    {
        repository = new InMemoryCheckPulseRepository();
    }

    [Fact]
    public async Task RetrieveVitalReadings_ShouldGetStoredVitals()
    {
        var retrieveVitalsResult = await repository.RetrieveVitalReadings();

        Assert.True(retrieveVitalsResult.IsSuccess);
        Assert.Equal("All", retrieveVitalsResult.Value[0]);
        Assert.Equal("Good", retrieveVitalsResult.Value[1]);
    }

    [Fact]
    public async Task SaveNewVitalCheck_ShouldSaveAndSucceed()
    {
        var savingResult = await repository.SaveNewVitalCheck();

        Assert.True(savingResult.IsSuccess);
    }
}